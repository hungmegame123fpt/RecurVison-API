using BusinessObject;
using BusinessObject.DTO.CV;
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
    public class CvVersionRepository : BaseRepository<CvVersion>, ICvVersionRepository
    {
        public CvVersionRepository(RecurVisionV1Context db) : base(db)
        {
        }
        public async Task<CvImportStatsDto> GetCvImportStatsAsync()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var weekAgo = now.AddDays(-7);
            var monthAgo = now.AddMonths(-1);

            return new CvImportStatsDto
            {
                Total = await _db.CvVersions.CountAsync(),
                ImportedToday = await _db.CvVersions.CountAsync(v => v.CreatedAt.HasValue && v.CreatedAt.Value.Date == today),
                ImportedThisWeek = await _db.CvVersions.CountAsync(v => v.CreatedAt.HasValue && v.CreatedAt.Value >= weekAgo),
                ImportedThisMonth = await _db.CvVersions.CountAsync(v => v.CreatedAt.HasValue && v.CreatedAt.Value >= monthAgo)
            };
        }
    }
}
