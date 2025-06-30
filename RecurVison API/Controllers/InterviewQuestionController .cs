using BusinessObject.DTO.InterviewQuestion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewQuestionController : ControllerBase
    {
        private readonly IInterviewQuestionService _questionService;
        private readonly ILogger<InterviewQuestionController> _logger;

        public InterviewQuestionController(
            IInterviewQuestionService questionService,
            ILogger<InterviewQuestionController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new interview question
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InterviewQuestionDto>> CreateQuestion([FromBody] CreateInterviewQuestionDto createDto)
        {
            try
            {
                var question = await _questionService.CreateQuestionAsync(createDto);
                return CreatedAtAction(nameof(GetQuestionById), new { id = question.QuestionId }, question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating interview question");
                return StatusCode(500, "An error occurred while creating the question");
            }
        }

        /// <summary>
        /// Get question by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InterviewQuestionDto>> GetQuestionById(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                    return NotFound($"Question with ID {id} not found");

                return Ok(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with ID {QuestionId}", id);
                return StatusCode(500, "An error occurred while retrieving the question");
            }
        }

        /// <summary>
        /// Get all questions for a specific interview
        /// </summary>
        [HttpGet("interview/{interviewId}")]
        public async Task<ActionResult<List<InterviewQuestionDto>>> GetQuestionsByInterviewId(int interviewId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByInterviewIdAsync(interviewId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for interview {InterviewId}", interviewId);
                return StatusCode(500, "An error occurred while retrieving questions");
            }
        }

        /// <summary>
        /// Get unanswered questions for an interview
        /// </summary>
        [HttpGet("interview/{interviewId}/unanswered")]
        public async Task<ActionResult<List<InterviewQuestionDto>>> GetUnansweredQuestions(int interviewId)
        {
            try
            {
                var questions = await _questionService.GetUnansweredQuestionsAsync(interviewId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unanswered questions for interview {InterviewId}", interviewId);
                return StatusCode(500, "An error occurred while retrieving questions");
            }
        }

        /// <summary>
        /// Update answer for a question
        /// </summary>
        [HttpPut("{id}/answer")]
        public async Task<ActionResult<InterviewQuestionDto>> UpdateAnswer(int id, [FromBody] UpdateAnswerDto updateDto)
        {
            try
            {
                var question = await _questionService.UpdateAnswerAsync(id, updateDto);
                return Ok(question);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Question with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating answer for question ID {QuestionId}", id);
                return StatusCode(500, "An error occurred while updating the answer");
            }
        }

        /// <summary>
        /// Update score and feedback for a question
        /// </summary>
        [HttpPut("{id}/score")]
        public async Task<ActionResult<InterviewQuestionDto>> UpdateQuestionScore(int id, [FromBody] UpdateQuestionScoreDto updateDto)
        {
            try
            {
                var question = await _questionService.UpdateQuestionScoreAsync(id, updateDto);
                return Ok(question);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Question with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating score for question ID {QuestionId}", id);
                return StatusCode(500, "An error occurred while updating the score");
            }
        }

        /// <summary>
        /// Delete a question
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            try
            {
                var deleted = await _questionService.DeleteQuestionAsync(id);
                if (!deleted)
                    return NotFound($"Question with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question with ID {QuestionId}", id);
                return StatusCode(500, "An error occurred while deleting the question");
            }
        }
    }
}
