using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CreateBlogPostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? Position { get; set; }
        public IFormFile? Avatar { get; set; }
        public IFormFile? ThumbnailImageUrl { get; set; } 
        public List<string> Categories { get; set; } = new();
    }
}
