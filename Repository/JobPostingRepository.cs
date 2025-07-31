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
    public class JobPostingRepository : BaseRepository<JobPosting>, IJobPostingRepository
    {
        public JobPostingRepository(RecurVisionV1Context db) : base(db)
        {

        }
        public async Task<JobPosting> GetByUserIdAsync(int userId)
        {
            return await _db.JobPostings
                .Where(j => j.UserId == userId)
                .Include(j => j.JobField) 
                .FirstOrDefaultAsync();
        }
        public async Task<JobPosting?> GetByTitleAndCompanyAsync(string jobTitle, string companyName)
        {
            return await _db.JobPostings
                .FirstOrDefaultAsync(j => j.JobPosition == jobTitle && j.CompanyName == companyName);
        }
    }
}
