using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _contactService.SubmitMessageAsync(dto);
            return Ok(new { message = "Message sent successfully" });
        }
        [HttpPost("reply")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReplyToContact([FromBody] ContactReplyDto dto)
        {
            var success = await _contactService.RespondToContactAsync(dto);
            if (!success) return NotFound(new { message = "Contact message not found" });
            return Ok(new { message = "Response sent successfully" });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllContacts()
        {
            var messages = await _contactService.GetAllContactsAsync();
            return Ok(messages);
        }
    }
}
