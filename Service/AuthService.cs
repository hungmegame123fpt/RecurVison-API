using BusinessObject.DTO;
using BusinessObject.Entities;
using DocumentFormat.OpenXml.Wordprocessing;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IOTPService _otpService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IOTPService otpService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _otpService = otpService;
        }

        public async Task<APIResponse<User>> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                if (await _unitOfWork.UserRepository.EmailExistsAsync(registerDto.Email))
                {
                    return new APIResponse<User>
                    {
                        Success = false,
                        Message = "Email already exists",
                        Errors = new List<string> { "User with this email already exists" }
                    };
                }

                var user = new User
                {
                    Email = registerDto.Email.ToLower(),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Password = HashPassword(registerDto.Password),
                    AccountStatus = "Active",
                    EmailVerified = false,
                };

                var createdUser = await _unitOfWork.UserRepository.CreateAsync(user);
                var userRole = new UserRole
                {
                    UserId = createdUser.UserId,
                    RoleId = 2,                      
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = null,              
                    IsPrimary = true
                };

                // Save UserRole
                await _unitOfWork.UserRoleRepository.CreateAsync(userRole);
                return new APIResponse<User>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = createdUser
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<User>
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse<User>> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginDto.Email.ToLower());

                if (user == null || !VerifyPassword(loginDto.Password, user.Password))
                {
                    return new APIResponse<User>
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                user.LastLogin = DateTime.UtcNow;
                await _unitOfWork.UserRepository.UpdateAsync(user);

                return new APIResponse<User>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<User>
                {
                    Success = false,
                    Message = "Login failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse<User>> GoogleAuthAsync(AuthDTO googleAuthDto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleAuthDto.IdToken);

                var existingUser = await _unitOfWork.UserRepository.GetByGoogleIdAsync(payload.Subject);
                if (existingUser != null)
                {
                    existingUser.EmailVerified = payload.EmailVerified;
                    existingUser.LastLogin = DateTime.UtcNow;
                    await _unitOfWork.UserRepository.UpdateAsync(existingUser);

                    return new APIResponse<User>
                    {
                        Success = true,
                        Message = "Google authentication successful",
                        Data = existingUser
                    };
                }

                var emailUser = await _unitOfWork.UserRepository.GetByEmailAsync(payload.Email);
                if (emailUser != null)
                {
                    emailUser.GoogleId = payload.Subject;
                    emailUser.EmailVerified = payload.EmailVerified;
                    emailUser.LastLogin = DateTime.UtcNow;
                    await _unitOfWork.UserRepository.UpdateAsync(emailUser);

                    return new APIResponse<User>
                    {
                        Success = true,
                        Message = "Google account linked successfully",
                        Data = emailUser
                    };
                }

                var newUser = new User
                {
                    Email = payload.Email.ToLower(),
                    FirstName = payload.GivenName ?? "",
                    LastName = payload.FamilyName ?? "",
                    GoogleId = payload.Subject,
                    EmailVerified = payload.EmailVerified,
                    AccountStatus = "Active",
                    LastLogin = DateTime.UtcNow,
                    
                };

                var createdUser = await _unitOfWork.UserRepository.CreateAsync(newUser);

                return new APIResponse<User>
                {
                    Success = true,
                    Message = "Google registration successful",
                    Data = createdUser
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<User>
                {
                    Success = false,
                    Message = "Google authentication failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse<string>> GenerateOtpAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByEmailAsync(email.ToLower());
                if (user == null)
                {
                    return new APIResponse<string>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var otp = await _otpService.GenerateOTPAsync(email);

                // Here you would typically send the OTP via email/SMS
                // For demo purposes, we'll return it in the response

                return new APIResponse<string>
                {
                    Success = true,
                    Message = "OTP generated successfully",
                    Data = otp // In production, don't return OTP in response
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<string>
                {
                    Success = false,
                    Message = "OTP generation failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse<bool>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            try
            {
                var isValid = await _otpService.VerifyOTPAsync(verifyOtpDto.Email.ToLower(), verifyOtpDto.Code);

                if (isValid)
                {
                    var user = await _unitOfWork.UserRepository.GetByEmailAsync(verifyOtpDto.Email.ToLower());
                    if (user != null)
                    {
                        user.EmailVerified = true;
                        await _unitOfWork.UserRepository.UpdateAsync(user);
                    }
                }

                return new APIResponse<bool>
                {
                    Success = isValid,
                    Message = isValid ? "OTP verified successfully" : "Invalid OTP",
                    Data = isValid
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>
                {
                    Success = false,
                    Message = "OTP verification failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new APIResponse<bool>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                if (!VerifyPassword(changePasswordDto.CurrentPassword, user.Password))
                {
                    return new APIResponse<bool>
                    {
                        Success = false,
                        Message = "Current password is incorrect"
                    };
                }

                user.Password = HashPassword(changePasswordDto.NewPassword);
                await _unitOfWork.UserRepository.UpdateAsync(user);

                return new APIResponse<bool>
                {
                    Success = true,
                    Message = "Password changed successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>
                {
                    Success = false,
                    Message = "Password change failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse<User?>> GetUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                return new APIResponse<User?>
                {
                    Success = true,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new APIResponse<User?>
                {
                    Success = false,
                    Message = "Failed to get user",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var salt = Guid.NewGuid().ToString();
                var saltedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes) + ":" + salt;
            }
        }

        private bool VerifyPassword(string password, string? hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return false;

            var parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;

            var hash = parts[0];
            var salt = parts[1];

            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                var newHash = Convert.ToBase64String(hashedBytes);
                return hash == newHash;
            }
        }
    }
}
