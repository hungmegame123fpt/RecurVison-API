using BusinessObject.Entities;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IBlogPostRepository : IBaseRepository<BlogPost>
    {
        Task<IEnumerable<BlogPost>> GetAllWithCategoriesAsync();
        Task<BlogPost?> GetByIdWithCategoriesAsync(int id);
    }
}
