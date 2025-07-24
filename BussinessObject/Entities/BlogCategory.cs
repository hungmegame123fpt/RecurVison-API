using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class BlogCategory
    {
        public int BlogCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }

}
