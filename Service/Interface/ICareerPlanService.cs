using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICareerPlanService
    {
        Task<NextCareerGoalDto?> GetNextCareerGoalAsync(int userId);
        Task<bool> UpdateCareerPlanAsync(int userId, UpdateCareerPlanRequest request);
        Task<bool> DeleteCareerPlanAsync(int planId);
        Task<List<CareerMilestoneResponse>> GetByPlanIdAsync(int planId);
    }
}
