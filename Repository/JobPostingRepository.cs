using BusinessObject;
using BusinessObject.Entities;
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
    }
}
