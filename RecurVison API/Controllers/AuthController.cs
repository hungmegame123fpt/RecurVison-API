using BusinessObject.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IOTPService _otpService;
        private readonly IUserRoleService _roleService;

        public AuthController(IAuthService authService, IEmailService emailService, IOTPService otpService, IUserRoleService roleService)
        {
            _authService = authService;
            _emailService = emailService;
            _otpService = otpService;
            _roleService = roleService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result.Success && result.Data != null)
            {
                var user = result.Data;
                var role = await _roleService.CheckRole(user);
                // Set authentication cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString()),
                    new Claim(ClaimTypes.Email, result.Data.Email),
                    new Claim(ClaimTypes.Name, $"{result.Data.FirstName} {result.Data.LastName}")
                };
                if (!string.IsNullOrEmpty(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });
                var account = result.Data;
                if (account.EmailVerified == false)
                {
                    var otpResponse = await _authService.GenerateOtpAsync(account.Email);
                    var otp = otpResponse.Data;
                    await _otpService.StoreOTPAsync(account.Email, otp);
                    await _emailService.SendOtpMail(account.Email, account.FirstName + " " + account.LastName, otp);

                    return Ok(new
                    {
                        Message = "OTP has been sent to your email. Please check your inbox for verification.",
                        IsSentMail = true
                    });
                }
                return Ok(new
                {
                    Result = result,
                    Token = claims
                });
            }

            return BadRequest(result);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth([FromBody] AuthDTO googleAuthDto)
        {
            var result = await _authService.GoogleAuthAsync(googleAuthDto);

            if (result.Success && result.Data != null)
            {
                var user = result.Data;
                var role = await _roleService.CheckRole(user);
                // Set authentication cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString()),
                    new Claim(ClaimTypes.Email, result.Data.Email),
                    new Claim(ClaimTypes.Name, $"{result.Data.FirstName} {result.Data.LastName}")
                };
                if (!string.IsNullOrEmpty(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

                return Ok(result);
            }

            return BadRequest(result);
        }
        [HttpGet("cookie-info")]
        public IActionResult GetCookieInfo()
        {
            var cookies = Request.Cookies.Select(c => new { c.Key, c.Value }).ToList();
            var cookieHeader = Request.Headers["Cookie"].ToString();

            return Ok(new
            {
                Cookies = cookies,
                CookieHeader = cookieHeader,
                IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false,
                AuthType = HttpContext.User.Identity?.AuthenticationType
            });
        }
        [HttpPost("generate-otp")]
        public async Task<IActionResult> GenerateOtp([FromBody] string email)
        {
            var result = await _authService.GenerateOtpAsync(email);
            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var result = await _authService.VerifyOtpAsync(verifyOtpDto);
            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return Ok(new { Success = true, Message = "Logged out successfully" });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _authService.GetUserAsync(userId);
            return Ok(result);
        }
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _authService.UpdateUserProfileAsync(userId, request);
                return Ok(new { Success = true, Message = "Update profile successfully" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
            
        }
    }
}
