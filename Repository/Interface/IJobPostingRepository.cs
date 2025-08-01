﻿using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IJobPostingRepository : IBaseRepository<JobPosting>
    {
        Task<JobPosting> GetByUserIdAsync(int userId);
        Task<JobPosting?> GetByTitleAndCompanyAsync(string jobTitle, string companyName);
    }
}
