using BusinessObject.Entities;
using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BlogPostRepository : BaseRepository<BlogPost>, IBlogPostRepository
    {
        private readonly RecurVisionV1Context _context;

        public BlogPostRepository(RecurVisionV1Context context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogPost>> GetAllWithCategoriesAsync()
        {
            return await _context.BlogPosts.Include(p => p.Categories).Include(p => p.Author).ToListAsync();
        }

        public async Task<BlogPost?> GetByIdWithCategoriesAsync(int id)
        {
            return await _context.BlogPosts.Include(p => p.Categories).Include(p => p.Author)
                                           .FirstOrDefaultAsync(p => p.BlogPostId == id);
        }
    }
}
