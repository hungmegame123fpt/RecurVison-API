using BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IUnitOfWork _unitOfWork;

        public BlogController(IBlogService blogService, IUnitOfWork unitOfWork)
        {
            _blogService = blogService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _blogService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _blogService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateBlogPostRequest request)
        {
          
            await _blogService.CreateAsync(request);
            return Ok(new { Success = true, Message = "Blog Create successfully" });
        }

        [HttpPost("upload-thumbnail")]
        public async Task<ActionResult> UploadThumbnail(IFormFile file)
        {
            var url = await _blogService.UploadThumbnailAsync(file);
            return Ok(new { url });
        }
        // GET: /api/authors
        [HttpGet("authors")]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _unitOfWork.AuthorRepository.GetAllAsync();
            return Ok(authors); // return list of { id, name }
        }

        // GET: /api/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _unitOfWork.BlogCategoryRepository.GetAllAsync();
            return Ok(categories); // return list of { id, name }
        }
    }
}
