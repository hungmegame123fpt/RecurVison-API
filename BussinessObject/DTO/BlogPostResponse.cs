using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class BlogPostResponse
    {
        public int BlogPostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string ThumbnailImageUrl { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public DateTime PublishedDate { get; set; }
    }
}
