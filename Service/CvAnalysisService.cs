using BusinessObject.Entities;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CvAnalysisService : ICvAnalysisResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CvAnalysisService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CvAnalysisResult?> GetLatestAnalysisByCvIdAsync(int cvId)
        {
            return await _unitOfWork.CvAnalysisResult.GetLatestAnalysisForCvAsync(cvId);
        }
    }
}
