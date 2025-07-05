using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.VirtualInterview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Security.Claims;

namespace RecurVison_API.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly IVirtualInterviewService _interviewService;
        private readonly ILogger<InterviewController> _logger;

        public InterviewController(
            IVirtualInterviewService interviewService,
            ILogger<InterviewController> logger)
        {
            _interviewService = interviewService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new virtual interview
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VirtualInterviewDto>> CreateInterview([FromBody] CreateVirtualInterviewDto createDto)
        {
            try
            {
                var interview = await _interviewService.CreateInterviewAsync(createDto);
                return CreatedAtAction(nameof(GetInterviewById), new { id = interview.InterviewId }, interview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating virtual interview");
                return StatusCode(500, "An error occurred while creating the interview");
            }
        }

        /// <summary>
        /// Get interview by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VirtualInterviewDto>> GetInterviewById(int id)
        {
            try
            {
                var interview = await _interviewService.GetInterviewByIdAsync(id);
                if (interview == null)
                    return NotFound($"Interview with ID {id} not found");

                return Ok(interview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interview with ID {InterviewId}", id);
                return StatusCode(500, "An error occurred while retrieving the interview");
            }
        }

        /// <summary>
        /// Get interview with all questions
        /// </summary>
        [HttpGet("{id}/with-questions")]
        public async Task<ActionResult<VirtualInterviewDto>> GetInterviewWithQuestions(int id)
        {
            try
            {
                var interview = await _interviewService.GetInterviewWithQuestionsAsync(id);
                if (interview == null)
                    return NotFound($"Interview with ID {id} not found");

                return Ok(interview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interview with questions for ID {InterviewId}", id);
                return StatusCode(500, "An error occurred while retrieving the interview");
            }
        }

        /// <summary>
        /// Get all interviews for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<VirtualInterviewDto>>> GetInterviewsByUserId(int userId)
        {
            try
            {
                var interviews = await _interviewService.GetInterviewsByUserIdAsync(userId);
                return Ok(interviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interviews for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving interviews");
            }
        }

        /// <summary>
        /// Get all interviews for a specific job
        /// </summary>
        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<List<VirtualInterviewDto>>> GetInterviewsByJobId(int jobId)
        {
            try
            {
                var interviews = await _interviewService.GetInterviewsByJobIdAsync(jobId);
                return Ok(interviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interviews for job {JobId}", jobId);
                return StatusCode(500, "An error occurred while retrieving interviews");
            }
        }

        /// <summary>
        /// Get interviews by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<VirtualInterviewDto>>> GetInterviewsByStatus(string status)
        {
            try
            {
                var interviews = await _interviewService.GetInterviewsByStatusAsync(status);
                return Ok(interviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interviews with status {Status}", status);
                return StatusCode(500, "An error occurred while retrieving interviews");
            }
        }

        /// <summary>
        /// Update interview status and score
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<VirtualInterviewDto>> UpdateInterviewStatus(int id, [FromBody] UpdateInterviewStatusDto updateDto)
        {
            try
            {
                var interview = await _interviewService.UpdateInterviewStatusAsync(id, updateDto);
                return Ok(interview);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Interview with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating interview status for ID {InterviewId}", id);
                return StatusCode(500, "An error occurred while updating the interview");
            }
        }

        /// <summary>
        /// Delete an interview
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInterview(int id)
        {
            try
            {
                var deleted = await _interviewService.DeleteInterviewAsync(id);
                if (!deleted)
                    return NotFound($"Interview with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting interview with ID {InterviewId}", id);
                return StatusCode(500, "An error occurred while deleting the interview");
            }
        }
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartInterviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

           request.UserId = int.Parse(userId);
            var result = await _interviewService.StartInterviewAsync(request);
            return Ok(result);
        }
        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswerAsync([FromBody] SubmitAnswerRequest request)
        {
            try
            {
                var result = await _interviewService.SubmitAnswerAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting interview answer.");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

