using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface  ICvAnalysisResultService
    {
        Task<CvAnalysisResultDto?> GetLatestAnalysisByCvIdAsync(int cvId);
        Task<List<CvAnalysisSummaryDto>> GetAnalysisSummariesAsync(int userId);
    }
}
