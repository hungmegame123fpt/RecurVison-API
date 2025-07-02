using BusinessObject.DTO.CvVersion;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CvVersionController : ControllerBase
    {
        private readonly ICvVersionService _cvVersionService;

        public CvVersionController(ICvVersionService cvVersionService)
        {
            _cvVersionService = cvVersionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var versions = await _cvVersionService.GetAllAsync();
            return Ok(versions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var version = await _cvVersionService.GetByIdAsync(id);
            if (version == null) return NotFound();
            return Ok(version);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CvVersionDTO version)
        {
            var created = await _cvVersionService.CreateAsync(version);
            return CreatedAtAction(nameof(GetById), new { id = created.VersionId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CvVersionDTO version)
        {
            if (id != version.VersionId) return BadRequest("Version ID mismatch");
            var existing = await _cvVersionService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var updated = await _cvVersionService.UpdateAsync(version);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _cvVersionService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _cvVersionService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/plaintext")]
        public async Task<IActionResult> UpdatePlainText(int id, [FromBody] string plainText)
        {
            var updated = await _cvVersionService.UpdatePlainTextAsync(id, plainText);
            return Ok(updated);
        }
    }
}
