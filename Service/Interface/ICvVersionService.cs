using BusinessObject.DTO.CvVersion;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICvVersionService
    {
        Task<IEnumerable<CvVersionDTO>> GetAllAsync();
        Task<CvVersionDTO?> GetByIdAsync(int id);
        Task<CvVersionDTO> CreateAsync(CvVersionDTO version);
        Task<CvVersionDTO> UpdateAsync(CvVersionDTO version);
        Task DeleteAsync(int id);
        Task<CvVersionDTO> UpdatePlainTextAsync(int id, string plainText);
    }
}
