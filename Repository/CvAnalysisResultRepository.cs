using BusinessObject;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
	public class CvAnalysisResultRepository : BaseRepository<CvAnalysisResult>, ICvAnalysisResultRepository
	{
		public CvAnalysisResultRepository(RecurVisionV1Context db) : base(db)
		{

		}
        public async Task<CvAnalysisResult?> GetLatestAnalysisForCvAsync(int cvId)
        {
            return await _db.CvAnalysisResults
                .Where(r => r.CvId == cvId)
                .Include(r => r.Skills)
                .Include(r => r.Education)
                .Include(r => r.Projects).ThenInclude(p => p.TechStacks)
                .Include(r => r.Certifications)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetCvsAnalyzedThisMonthAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _db.CvAnalysisResults
                .Include(r => r.Cv)
                .CountAsync(c => c.Cv.UserId == userId &&
                c.CreatedAt.Month == now.Month &&
                c.CreatedAt.Year == now.Year);
        }
        public IQueryable<CvAnalysisResult> GetQueryable()
        {
            return _db.Set<CvAnalysisResult>().AsQueryable();
        }
    }
}
