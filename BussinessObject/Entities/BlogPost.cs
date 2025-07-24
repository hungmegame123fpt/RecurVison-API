using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class BlogPost
    {
        public int BlogPostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string ThumbnailImageUrl { get; set; } = string.Empty;

        public int AuthorId { get; set; }
        public virtual Author Author { get; set; } = null!;

        public virtual ICollection<BlogCategory> Categories { get; set; } = new List<BlogCategory>();
    }
}
