using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ICVRepository : IBaseRepository<Cv>
    {
        Task<List<CVDto>> GetAllAsync();
        Task<Cv?> GetByIdAsync(int cvId);
        Task<Cv?> GetByUserIdAsync(int userId, int cvId);
        Task<List<Cv>> GetByUserIdAsync(int userId);
        Task<Cv> CreateAsync(Cv cv);
        Task<Cv> UpdateAsync(Cv cv);
        Task<bool> DeleteAsync(int cvId);
        Task<bool> ExistsAsync(int cvId);
        Task<CvVersion?> GetLatestVersionAsync(int cvId);
        Task<CvVersion> CreateVersionAsync(CvVersion version);
        Task<int?> CategorizeCvByFieldAsync(int cvId, string plainTextContent);
        Task MatchKeywordsAndSaveAsync(int cvId, string plainTextCv);
    }
}
