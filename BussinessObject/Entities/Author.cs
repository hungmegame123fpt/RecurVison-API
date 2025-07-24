using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Position { get; set; } = string.Empty; // e.g., "Head of Technology"
        public string? ProfileImageUrl { get; set; } = string.Empty;

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}
