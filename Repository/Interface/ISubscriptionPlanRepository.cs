using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ISubscriptionPlanRepository : IBaseRepository<SubscriptionPlan>
    {
        Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync();
        Task<IEnumerable<SubscriptionPlan>> GetPlansByUserTypeAsync(string userType);
        Task<SubscriptionPlan?> GetPlanByNameAsync(string planName);
        Task<IEnumerable<SubscriptionPlan>> GetPlansByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<SubscriptionPlan>> GetPlansByBillingCycleAsync(string billingCycle);
        Task<bool> DeactivatePlanAsync(int planId);
        Task<bool> ActivatePlanAsync(int planId);
        Task<int> GetActiveSubscriptionCountAsync(int planId);
        Task<IEnumerable<SubscriptionPlan>> SearchPlansAsync(string searchTerm);
        Task<IEnumerable<SubscriptionPlan>> GetAllAsync();
    }
}
