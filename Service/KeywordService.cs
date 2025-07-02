using BusinessObject.Entities;
using Repository;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class KeywordService : IKeywordService
    {
        private readonly IUnitOfWork _unitOfWork;
        public KeywordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Keyword>> GetAllKeywordsAsync(string? search = null)
        {
            return await _unitOfWork.KeywordRepository.GetAllAsync(search);
        }

        public async Task<Keyword?> GetKeywordByIdAsync(int id)
        {
            return await _unitOfWork.KeywordRepository.GetByIdAsync(id);
        }

        public async Task<Keyword> CreateKeywordAsync(Keyword keyword)
        {
            await _unitOfWork.KeywordRepository.CreateAsync(keyword);
            await _unitOfWork.SaveChanges();
            return keyword;
        }

        public async Task<Keyword> UpdateKeywordAsync(Keyword keyword)
        {
            await _unitOfWork.KeywordRepository.UpdateAsync(keyword);
            await _unitOfWork.SaveChanges();
            return keyword;
        }

        public async Task DeleteKeywordAsync(int id)
        {
            var keyword = await _unitOfWork.KeywordRepository.GetByIdAsync(id);
            if (keyword != null)
            {
                await _unitOfWork.KeywordRepository.DeleteAsync(keyword);
                await _unitOfWork.SaveChanges();
            }
        }
    }
}
