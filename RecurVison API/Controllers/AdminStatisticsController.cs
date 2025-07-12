using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStatisticsController : ControllerBase
    {
        private readonly IAdminStatisticsService _statisticsService;

        public AdminStatisticsController(IAdminStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("cvs/new-today")]
        public async Task<IActionResult> GetNewCvsToday()
        {
            var count = await _statisticsService.GetNewCvsTodayAsync();
            return Ok(count);
        }

        [HttpGet("interviews/new-today")]
        public async Task<IActionResult> GetNewInterviewsToday()
        {
            var count = await _statisticsService.GetNewInterviewsTodayAsync();
            return Ok(count);
        }

        [HttpGet("users/new-today")]
        public async Task<IActionResult> GetNewUsersToday()
        {
            var count = await _statisticsService.GetNewUsersTodayAsync();
            return Ok(count);
        }

        [HttpGet("cvs/total")]
        public async Task<IActionResult> GetTotalCvs()
        {
            var total = await _statisticsService.GetTotalCvsAsync();
            return Ok(total);
        }

        [HttpGet("cv-fields/total")]
        public async Task<IActionResult> GetTotalCvFields()
        {
            var fields = await _statisticsService.GetTotalCvFieldsAsync();
            return Ok(fields);
        }

        [HttpGet("interviews/total")]
        public async Task<IActionResult> GetTotalInterviews()
        {
            var total = await _statisticsService.GetTotalInterviewsAsync();
            return Ok(total);
        }

        [HttpGet("interviews/in-progress")]
        public async Task<IActionResult> GetInterviewsInProgress()
        {
            var count = await _statisticsService.GetInterviewsInProgressAsync();
            return Ok(count);
        }

        [HttpGet("cv-fields/distribution")]
        public async Task<IActionResult> GetCvFieldDistribution()
        {
            var result = await _statisticsService.GetCvFieldDistributionAsync();
            return Ok(result);
        }

        [HttpGet("cv-analysis/over-time")]
        public async Task<IActionResult> GetCvAnalysisOverTime()
        {
            var result = await _statisticsService.GetCvAnalysisOverTimeAsync();
            return Ok(result);
        }

        [HttpGet("interviews/over-time")]
        public async Task<IActionResult> GetInterviewCountOverTime()
        {
            var result = await _statisticsService.GetInterviewCountOverTimeAsync();
            return Ok(result);
        }

        [HttpGet("analysis/score-distribution")]
        public async Task<IActionResult> GetScoreDistribution()
        {
            var result = await _statisticsService.GetScoreDistributionAsync();
            return Ok(result);
        }
    }
}
