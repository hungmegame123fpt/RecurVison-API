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
    public class CareerPlanRepository : BaseRepository<CareerPlan>, ICareerPlanRepository
    {
        public CareerPlanRepository(RecurVisionV1Context db) : base(db)
        {
        }
        public async Task<CareerPlan?> GetCareerPlanByUserIdAsync(int userId)
        {
            return await _db.CareerPlans
                .Include(p => p.CareerMilestones)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
