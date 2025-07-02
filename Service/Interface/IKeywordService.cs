using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IKeywordService
    {
        Task<IEnumerable<Keyword>> GetAllKeywordsAsync(string? search = null);
        Task<Keyword?> GetKeywordByIdAsync(int id);
        Task<Keyword> CreateKeywordAsync(Keyword keyword);
        Task<Keyword> UpdateKeywordAsync(Keyword keyword);
        Task DeleteKeywordAsync(int id);
    }
}
