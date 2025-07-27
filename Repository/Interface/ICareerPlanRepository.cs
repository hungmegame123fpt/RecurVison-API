using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ICareerPlanRepository : IBaseRepository<CareerPlan>
    {
        Task<CareerPlan?> GetCareerPlanByUserIdAsync(int userId);
        Task<List<CareerMilestone>> GetByPlanIdAsync(int planId);
    }
}
