using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobMatchingController : ControllerBase
    {
        private readonly IAIClient _client;

        public JobMatchingController(IAIClient client)
        {
            _client = client;
        }
        [HttpGet("ai/suggested-jobs/{cvId}")]
        public async Task<IActionResult> GetSuggestedJobs(int cvId)
        {
             var jobs = await _client.GetSuggestedJobsForCvAsync(cvId);
             return Ok(jobs);   
        }
    }
}
