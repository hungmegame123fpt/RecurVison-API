using AutoMapper;
using BusinessObject.DTO;
using BusinessObject.DTO.SubscriptionPlan;
using BusinessObject.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionPlanService> _logger;

        public SubscriptionPlanService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubscriptionPlanService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);

            // Populate active subscription count for each plan
            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetActivePlansAsync()
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetActivePlansAsync();
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);

            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);
            if (plan == null)
                return null;

            var planDto = _mapper.Map<SubscriptionPlanDto>(plan);
            planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planId);

            return planDto;
        }

        public async Task<SubscriptionPlanDto?> GetPlanByNameAsync(string planName)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetPlanByNameAsync(planName);
            if (plan == null)
                return null;

            var planDto = _mapper.Map<SubscriptionPlanDto>(plan);
            planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(plan.PlanId);

            return planDto;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetPlansByUserTypeAsync(string userType)
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetPlansByUserTypeAsync(userType);
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);

            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetPlansByBillingCycleAsync(string billingCycle)
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetPlansByBillingCycleAsync(billingCycle);
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);

            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetPlansByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice > maxPrice)
            {
                throw new ArgumentException("Minimum price cannot be greater than maximum price");
            }

            var plans = await _unitOfWork.SubscriptionPlanRepository.GetPlansByPriceRangeAsync(minPrice, maxPrice);
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);

            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> SearchPlansAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetActivePlansAsync();
            }

            var plans = await _unitOfWork.SubscriptionPlanRepository.SearchPlansAsync(searchTerm);
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);

            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetFilteredPlansAsync(PlanFilterDto filter)
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();
            var query = plans.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.UserType))
            {
                query = query.Where(p => p.UserType.Equals(filter.UserType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filter.BillingCycle))
            {
                query = query.Where(p => p.BillingCycle.Equals(filter.BillingCycle, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(p => p.PlanName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                       p.Features.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "planname" => filter.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(p => p.PlanName)
                    : query.OrderBy(p => p.PlanName),
                "createdat" => filter.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                _ => filter.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price)
            };

            var filteredPlans = query.ToList();
            var planDtos = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(filteredPlans);

            foreach (var planDto in planDtos)
            {
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planDto.PlanId);
            }

            return planDtos;
        }

        public async Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createDto)
        {
            // Check if plan name already exists
            var existingPlan = await _unitOfWork.SubscriptionPlanRepository.GetPlanByNameAsync(createDto.PlanName);
            if (existingPlan != null)
            {
                throw new ValidationException($"A plan with the name '{createDto.PlanName}' already exists");
            }

            var plan = _mapper.Map<SubscriptionPlan>(createDto);
            plan.CreatedAt = DateTime.UtcNow;
            plan.IsActive = true;

            var createdPlan = await _unitOfWork.SubscriptionPlanRepository.CreateAsync(plan);
            await _unitOfWork.SubscriptionPlanRepository.SaveChangesAsync();

            _logger.LogInformation("Subscription plan '{PlanName}' created successfully with ID {PlanId}",
                createdPlan.PlanName, createdPlan.PlanId);

            var planDto = _mapper.Map<SubscriptionPlanDto>(createdPlan);
            planDto.ActiveSubscriptionCount = 0;

            return planDto;
        }

        public async Task<SubscriptionPlanDto> UpdatePlanAsync(int planId, UpdateSubscriptionPlanDto updateDto)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);
            // Check if plan name already exists (if being updated)
            if (!string.IsNullOrEmpty(updateDto.PlanName) && updateDto.PlanName != plan.PlanName)
            {
                var existingPlan = await _unitOfWork.SubscriptionPlanRepository.GetPlanByNameAsync(updateDto.PlanName);
                if (existingPlan != null && existingPlan.PlanId != planId)
                {
                    throw new ValidationException($"A plan with the name '{updateDto.PlanName}' already exists");
                }
            }

            _mapper.Map(updateDto, plan);
            plan.UpdatedAt = DateTime.UtcNow;

            var updatedPlan = await _unitOfWork.SubscriptionPlanRepository.UpdateAsync(plan);
            await _unitOfWork.SubscriptionPlanRepository.SaveChangesAsync();

            _logger.LogInformation("Subscription plan {PlanId} updated successfully", planId);

            var planDto = _mapper.Map<SubscriptionPlanDto>(updatedPlan);
            planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planId);

            return planDto;
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);
            if (plan == null)
            {
                return false;
            }

            // Check if plan can be deleted
            if (!await CanDeletePlanAsync(planId))
            {
                throw new ValidationException("Cannot delete plan with active subscriptions. Please deactivate the plan instead.");
            }

            await _unitOfWork.SubscriptionPlanRepository.DeleteAsync(plan);
            await _unitOfWork.SubscriptionPlanRepository.SaveChangesAsync();

            _logger.LogInformation("Subscription plan {PlanId} deleted successfully", planId);
            return true;
        }

        public async Task<bool> ActivatePlanAsync(int planId)
        {
            var result = await _unitOfWork.SubscriptionPlanRepository.ActivatePlanAsync(planId);
            if (result)
            {
                _logger.LogInformation("Subscription plan {PlanId} activated successfully", planId);
            }
            return result;
        }

        public async Task<bool> DeactivatePlanAsync(int planId)
        {
            var result = await _unitOfWork.SubscriptionPlanRepository.DeactivatePlanAsync(planId);
            if (result)
            {
                _logger.LogInformation("Subscription plan {PlanId} deactivated successfully", planId);
            }
            return result;
        }

        public async Task<PlanStatsDto> GetPlanStatsAsync()
        {
            var allPlans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();
            var stats = new PlanStatsDto
            {
                TotalPlans = allPlans.Count(),
                ActivePlans = allPlans.Count(p => (bool)p.IsActive),
                InactivePlans = allPlans.Count(p => (bool)!p.IsActive)
            };

            // Calculate subscription counts and revenue
            foreach (var plan in allPlans)
            {
                var subscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(plan.PlanId);
                stats.TotalActiveSubscriptions += subscriptionCount;

                var revenue = subscriptionCount * plan.Price;
                if (plan.BillingCycle.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    stats.TotalMonthlyRevenue += revenue;
                }
                else if (plan.BillingCycle.Equals("Yearly", StringComparison.OrdinalIgnoreCase))
                {
                    stats.TotalYearlyRevenue += revenue;
                }

                stats.MostPopularPlans.Add(new PlanPopularityDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    SubscriptionCount = subscriptionCount,
                    Revenue = revenue
                });
            }

            // Sort by subscription count to get most popular plans
            stats.MostPopularPlans = stats.MostPopularPlans
                .OrderByDescending(p => p.SubscriptionCount)
                .Take(5)
                .ToList();

            return stats;
        }

        public async Task<bool> PlanExistsAsync(int planId)
        {
            return await _unitOfWork.SubscriptionPlanRepository.ExistsAsync(planId);
        }

        public async Task<bool> CanDeletePlanAsync(int planId)
        {
            var activeSubscriptions = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(planId);
            return activeSubscriptions == 0;
        }
    }
}