using BusinessObject.DTO;
using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;
using System.Security.Claims;

namespace RecurVison_API.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CvController : ControllerBase
    {
        private readonly ICVService _cvService;
        private readonly ICvAnalysisResultService _analysisService;
        private readonly IFileStorageService _storageService;
        private readonly ILogger<CvController> _logger;

        public CvController(ICVService cvService, ILogger<CvController> logger, IFileStorageService storageService, ICvAnalysisResultService analysisService)
        {
            _cvService = cvService;
            _logger = logger;
            _storageService = storageService;
            _analysisService = analysisService;
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
		[HttpPost("analyze")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CvAnalysisResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AnalyzeCV([FromForm] CvAnalysisResultRequest cvAnalysis)
		{
			if (cvAnalysis.jdFile == null || cvAnalysis.jdFile.Length == 0)
			{
				return BadRequest("Job description file is required.");
			}
			try
			{
				var result = await _cvService.AnalyzeCvAsync(cvAnalysis);

				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
			catch (FileNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred during analysis: {ex.Message}");
			}
		}
        [HttpGet("analysis-summaries")]
        [ProducesResponseType(typeof(List<CvAnalysisSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAnalysisSummaries()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            var results = await _analysisService.GetAnalysisSummariesAsync(userId);
            return Ok(results);
        }
        [HttpGet("latest-analysis/{cvId}")]
        [ProducesResponseType(typeof(CvAnalysisResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLatestAnalysis(int cvId)
        {
            var result = await _analysisService.GetLatestAnalysisByCvIdAsync(cvId);

            if (result == null)
            {
                return NotFound($"No analysis found for CV ID {cvId}");
            }

            return Ok(result);
        }
        [HttpPost("cv-version/{cvVersionId}/upload-analysis")]
        public async Task<IActionResult> UploadCvAnalysisJson([FromForm] CvAnalysisRequest request)
        {
            var result = await _cvService.ImportCvAnalysisJsonAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
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
        [HttpPost("parse")]
        [ProducesResponseType(typeof(ParsedDocumentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ParseCv([FromBody] ParseCvRequest request)
        {
            try
            {
                var result = await _cvService.ParseCvAsync(request.CvId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing CV");
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("edit")]
        public async Task<IActionResult> EditPdf([FromBody] EditPdfRequest request)
        {
            // Get original CV from cloudinary/local storage
            var cv = await _cvService.GetCvByIdAsync(request.CvId);
            var fileBytes = await _storageService.GetFileAsync(cv.Cv.FilePath);

            if (fileBytes == null) return NotFound("Original CV not found.");

            var updatedPdf = await _cvService.EditPdfAsync(fileBytes, request.NewText);

            return File(updatedPdf, "application/pdf", "Edited_CV.pdf");
        }
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<CVDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCvs()
        {
            var cvs = await _cvService.GetAllCvAsync();
            return Ok(cvs);
        }
        [HttpGet("admin/{userId}")]
        public async Task<ActionResult<CvListResponse>> GetUserCvs(int userId)
        {
            var response = await _cvService.GetUserCvsAsync(userId);
            return response.Success ? Ok(response) : StatusCode(500, response);
        }
        [HttpGet("user")]
        public async Task<ActionResult<CvListResponse>> GetUserCvs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var response = await _cvService.GetUserCvsAsync(int.Parse(userId));
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{cvId}")]
        public async Task<ActionResult<CvDetailResponse>> GetCvById(int cvId)
        {
            var response = await _cvService.GetCvByIdAsync(cvId);
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
        
        [HttpGet("JobField")]
        public async Task<ActionResult<string?>> CategorizeCvByFieldAsync(int cvId)
        {
            var result = await _cvService.CategorizeCvByFieldAsync(cvId);
            return Ok(result);
        }
    }
}
