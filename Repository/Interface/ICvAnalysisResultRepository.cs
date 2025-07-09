using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
	public interface ICvAnalysisResultRepository : IBaseRepository<CvAnalysisResult>
	{
        Task<CvAnalysisResult?> GetLatestAnalysisForCvAsync(int cvId);
        IQueryable<CvAnalysisResult> GetQueryable();
    }
}
