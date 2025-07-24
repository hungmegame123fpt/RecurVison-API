using BusinessObject.Entities;
using BusinessObject;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BlogCategoryRepository : BaseRepository<BlogCategory>, IBlogCategoryRepository
    {
        private readonly RecurVisionV1Context _context;

        public BlogCategoryRepository(RecurVisionV1Context context) : base(context)
        {
            _context = context;
        }

        public async Task<BlogCategory?> GetByNameAsync(string name)
        {
            return await _context.BlogCategories.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
