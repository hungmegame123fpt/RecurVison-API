using BusinessObject.DTO.SubscriptionPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ISubscriptionPlanService
    {
        Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();
        Task<IEnumerable<SubscriptionPlanDto>> GetActivePlansAsync();
        Task<SubscriptionPlanDto?> GetPlanByIdAsync(int planId);
        Task<SubscriptionPlanDto?> GetPlanByNameAsync(string planName);
        Task<IEnumerable<SubscriptionPlanDto>> GetPlansByUserTypeAsync(string userType);
        Task<IEnumerable<SubscriptionPlanDto>> GetPlansByBillingCycleAsync(string billingCycle);
        Task<IEnumerable<SubscriptionPlanDto>> GetPlansByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<SubscriptionPlanDto>> SearchPlansAsync(string searchTerm);
        Task<IEnumerable<SubscriptionPlanDto>> GetFilteredPlansAsync(PlanFilterDto filter);
        Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createDto);
        Task<SubscriptionPlanDto> UpdatePlanAsync(int planId, UpdateSubscriptionPlanDto updateDto);
        Task<bool> DeletePlanAsync(int planId);
        Task<bool> ActivatePlanAsync(int planId);
        Task<bool> DeactivatePlanAsync(int planId);
        Task<PlanStatsDto> GetPlanStatsAsync();
        Task<bool> PlanExistsAsync(int planId);
        Task<bool> CanDeletePlanAsync(int planId);
    }
}
