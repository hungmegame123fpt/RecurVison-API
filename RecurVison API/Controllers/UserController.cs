using BusinessObject.DTO.User;
using BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.ComponentModel.DataAnnotations;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetUsers([FromQuery] UserFilterDto filter)
        {
            try
            {
                var result = await _userService.GetUsersAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId:int}")]
        public async Task<ActionResult<UserDto>> GetUser(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{userId:int}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int userId, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(userId, updateDto);
                return Ok(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId:int}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId:int}/suspend")]
        public async Task<ActionResult> SuspendUser(int userId)
        {
            try
            {
                var result = await _userService.SuspendUserAsync(userId);
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                return Ok(new { message = "User suspended successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId:int}/activate")]
        public async Task<ActionResult> ActivateUser(int userId)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(userId);
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<UserStatsDto>> GetUserStats()
        {
            try
            {
                var stats = await _userService.GetUserStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user stats");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
