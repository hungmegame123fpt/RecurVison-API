using BusinessObject.DTO;
using BusinessObject.DTO.CV;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CvController : ControllerBase
    {
        private readonly ICVService _cvService;
        private readonly ILogger<CvController> _logger;

        public CvController(ICVService cvService, ILogger<CvController> logger)
        {
            _cvService = cvService;
            _logger = logger;
        }

        [HttpPost("import")]
        public async Task<ActionResult<CVImportResponse>> ImportCv([FromForm] CVImportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _cvService.ImportCvAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("export")]
        public async Task<IActionResult> ExportCv([FromBody] CVExportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _cvService.ExportCvAsync(request);

            if (result.Success && result.FileContent != null)
            {
                return File(result.FileContent, result.ContentType!, result.FileName);
            }

            return BadRequest(result);
        }
        [HttpPost("export-view")]
        [Produces("application/pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportCvInline([FromBody] CVExportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cvService.ExportCvAsync(request);

            if (result.Success && result.FileContent != null)
            {
                Response.Headers.Add("Content-Disposition", $"inline; filename={result.FileName}");
                return File(result.FileContent, "application/pdf");
            }

            return BadRequest(result);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CvListResponse>> GetUserCvs(int userId)
        {
            var response = await _cvService.GetUserCvsAsync(userId);
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{cvId}")]
        public async Task<ActionResult<CvDetailResponse>> GetCvById(int cvId, [FromQuery] int userId)
        {
            var response = await _cvService.GetCvByIdAsync(userId, cvId);
            return response.Success ? Ok(response) :
                   response.Message.Contains("not found") ? NotFound(response) :
                   StatusCode(500, response);
        }

        [HttpDelete("{cvId}")]
        public async Task<ActionResult<DeleteResponse>> DeleteCv(int cvId, [FromQuery] int userId)
        {
            var response = await _cvService.DeleteCvAsync(userId, cvId);
            return response.Success ? Ok(response) :
                   response.Message.Contains("not found") ? NotFound(response) :
                   StatusCode(500, response);
        }
        [HttpGet("formats/import")]
        public async Task<ActionResult<List<string>>> GetSupportedImportFormats()
        {
            var formats = await _cvService.GetSupportedImportFormats();
            return Ok(formats);
        }

        [HttpGet("formats/export")]
        public async Task<ActionResult<List<string>>> GetSupportedExportFormats()
        {
            var formats = await _cvService.GetSupportedExportFormats();
            return Ok(formats);
        }
    }
}
