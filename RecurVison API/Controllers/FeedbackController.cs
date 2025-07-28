using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;
using System.Security.Claims;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] CreateFeedbackRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            await _feedbackService.AddFeedbackAsync(request, userId);
            return Ok("Feedback submitted.");
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserFeedbacks()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            var data = await _feedbackService.GetFeedbacksByUserAsync(userId);
            return Ok(data);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _feedbackService.GetAllAsync();
            return Ok(result);
        }

        // User updates their feedback
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateFeedbackRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            await _feedbackService.UpdateAsync(userId, request);
            return Ok(new { message = "Updated successfully" });
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _feedbackService.GetRatingStatisticsAsync();
            return Ok(stats);
        }
    }
}
