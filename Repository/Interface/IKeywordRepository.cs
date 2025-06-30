using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IKeywordRepository : IBaseRepository<Keyword>
    {
        Task<string?> GetKeywordNameByIdAsync(int? id);
    }
}
