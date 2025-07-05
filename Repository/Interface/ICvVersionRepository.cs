using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ICvVersionRepository : IBaseRepository<CvVersion>

    {
        Task<CvImportStatsDto> GetCvImportStatsAsync();
    }
}
