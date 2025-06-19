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
    public class SubscriptionPlanRepository : BaseRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(RecurVisionV1Context context) : base(context) 
        {
        }
        public override async Task<SubscriptionPlan> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(p => p.UserSubscriptions)
                .FirstOrDefaultAsync(p => p.PlanId == id);
        }
        public  async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {   
                return await dbSet
                    .Include(p => p.UserSubscriptions)
                    .OrderBy(p => p.PlanName)
                    .ToListAsync();                    
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
                return await dbSet
                    .Where(p => p.IsActive == true)
                    .OrderBy(p => p.Price)
                    .ToListAsync();           
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetPlansByUserTypeAsync(string userType)
        {    
                return await dbSet
                    .Where(p => p.UserType == userType && p.IsActive == true)
                    .OrderBy(p => p.Price)
                    .ToListAsync();                      
        }

        public async Task<SubscriptionPlan?> GetPlanByNameAsync(string planName)
        {
            
                return await dbSet
                    .FirstOrDefaultAsync(p => p.PlanName.ToLower() == planName.ToLower());          
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetPlansByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
           
                return await dbSet
                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive == true)
                    .OrderBy(p => p.Price)
                    .ToListAsync();        
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetPlansByBillingCycleAsync(string billingCycle)
        {
          
                return await dbSet
                    .Where(p => p.BillingCycle == billingCycle && p.IsActive == true)
                    .OrderBy(p => p.Price)
                    .ToListAsync();   
           
        }

        public async Task<bool> DeactivatePlanAsync(int planId)
        {
            
                var plan = await GetByIdAsync(planId);
                if (plan == null)
                    return false;

                plan.IsActive = false;
                await UpdateAsync(plan);
                await SaveChangesAsync();
                return true;           
        }

        public async Task<bool> ActivatePlanAsync(int planId)
        {
           
                var plan = await GetByIdAsync(planId);
                if (plan == null)
                    return false;

                plan.IsActive = true;
                await UpdateAsync(plan);
                await SaveChangesAsync();
                return true;           
        }

        public async Task<int> GetActiveSubscriptionCountAsync(int planId)
        {
            
                return await _db.UserSubscriptions
                    .CountAsync(s => s.PlanId == planId &&
                                    s.PaymentStatus == "ACTIVE" &&
                                    s.EndDate > DateTime.UtcNow);         
        }

        public async Task<IEnumerable<SubscriptionPlan>> SearchPlansAsync(string searchTerm)
        {
                return await dbSet
                    .Where(p => p.PlanName.Contains(searchTerm) ||
                               p.Features.Contains(searchTerm) ||
                               p.UserType.Contains(searchTerm))
                    .Where(p => p.IsActive == true)
                    .OrderBy(p => p.PlanName)
                    .ToListAsync();            
        }
    }
}
