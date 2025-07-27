using BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Security.Claims;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareerPlanController : ControllerBase
    {
        private readonly ICareerPlanService _careerPlanService;

        public CareerPlanController(ICareerPlanService careerPlanService)
        {
            _careerPlanService = careerPlanService;
        }
        [HttpGet("user/next-career-goal")]
        public async Task<IActionResult> GetNextCareerGoal()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            var result = await _careerPlanService.GetNextCareerGoalAsync(userId);
            if (result == null)
                return NotFound("Career plan not found");

            return Ok(result);
        }
        [HttpPut("career-plan")]
        public async Task<IActionResult> UpdateCareerPlan([FromBody] UpdateCareerPlanRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            var success = await _careerPlanService.UpdateCareerPlanAsync(userId,request);
            return success ? Ok(new { message = "Career plan updated successfully." }) : BadRequest("Update failed.");
        }
        [HttpDelete("career-plan/{planId}")]
        public async Task<IActionResult> DeleteCareerPlan(int planId)
        {
            var success = await _careerPlanService.DeleteCareerPlanAsync(planId);
            if (!success) return NotFound("Career plan not found.");
            return Ok(new { message = "Career plan deleted successfully." });
        }
        [HttpGet("by-plan/{planId}")]
        public async Task<IActionResult> GetByPlanId(int planId)
        {
            var milestones = await _careerPlanService.GetByPlanIdAsync(planId);
            return Ok(milestones);
        }
    }
}
