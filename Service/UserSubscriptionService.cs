using AutoMapper;
using BusinessObject.DTO.UserSubscription;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionPlanService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserSubscriptionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubscriptionPlanService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> ExpireEndedSubscriptionsAsync()
        {
            return await _unitOfWork.UserSubscriptionRepository.ExpireEndedSubscriptionsAsync();
        }
        public async Task<UserSubscriptionDto?> GetUserActiveSubscriptionAsync(int userId)
        {
            try
            {
                var subscription = await _unitOfWork.UserSubscriptionRepository.GetUserActiveSubscriptionAsync(userId);
                return _mapper.Map<UserSubscriptionDto>(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription for user {UserId}", userId);
                throw;
            }
        }
        public async Task<UserSubscriptionDto> UpdateSubscriptionAsync(int id, UserSubscriptionDto subscriptionDto)
        {
            var existingSub = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
            if (existingSub == null)
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");

            _mapper.Map(subscriptionDto, existingSub);
            await _unitOfWork.UserSubscriptionRepository.UpdateAsync(existingSub);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<UserSubscriptionDto>(existingSub);
        }
        public async Task<List<UserSubscriptionDto>> GetUserSubscriptionHistoryAsync(int userId)
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetUserSubscriptionHistoryAsync(userId);
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription history for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserSubscriptionDto>> GetSubscriptionsByPlanAsync(int planId)
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionsByPlanAsync(planId);
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for plan {PlanId}", planId);
                throw;
            }
        }

        public async Task<List<UserSubscriptionDto>> GetExpiredSubscriptionsAsync()
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetExpiredSubscriptionsAsync();
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expired subscriptions");
                throw;
            }
        }

        public async Task<List<UserSubscriptionDto>> GetExpiringSubscriptionsAsync(int daysFromNow)
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetExpiringSubscriptionsAsync(daysFromNow);
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring subscriptions within {Days} days", daysFromNow);
                throw;
            }
        }

        public async Task<List<UserSubscriptionDto>> GetAutoRenewSubscriptionsAsync()
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetAutoRenewSubscriptionsAsync();
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting auto-renew subscriptions");
                throw;
            }
        }

        public async Task<UserSubscriptionDto?> GetSubscriptionByOrderCodeAsync(long orderCode)
        {
            try
            {
                var subscription = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionByOrderCodeAsync(orderCode);
                return _mapper.Map<UserSubscriptionDto>(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription by order code {OrderCode}", orderCode);
                throw;
            }
        }

        public async Task<bool> CancelSubscriptionAsync(int subscriptionId)
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.CancelSubscriptionAsync(subscriptionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling subscription {SubscriptionId}", subscriptionId);
                throw;
            }
        }

        public async Task<bool> RenewSubscriptionAsync(int subscriptionId, DateTime newEndDate)
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.RenewSubscriptionAsync(subscriptionId, newEndDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing subscription {SubscriptionId}", subscriptionId);
                throw;
            }
        }

        public async Task<List<UserSubscriptionDto>> GetSubscriptionsByStatusAsync(string status)
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionsByStatusAsync(status);
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions by status {Status}", status);
                throw;
            }
        }

        public async Task<List<UserSubscriptionDto>> GetSubscriptionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionsByDateRangeAsync(startDate, endDate);
                return _mapper.Map<List<UserSubscriptionDto>>(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions by date range {StartDate} - {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.GetTotalRevenueAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                throw;
            }
        }

        public async Task<decimal> GetRevenueByPlanAsync(int planId)
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.GetRevenueByPlanAsync(planId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue for plan {PlanId}", planId);
                throw;
            }
        }

        public async Task<int> GetActiveSubscriptionCountAsync()
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription count");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetSubscriptionStatsByStatusAsync()
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.GetSubscriptionStatsByStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription stats by status");
                throw;
            }
        }

        public async Task<UserSubscriptionDto?> GetSubscriptionWithDetailsAsync(int subscriptionId)
        {
            try
            {
                var subscription = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionWithDetailsAsync(subscriptionId);
                return _mapper.Map<UserSubscriptionDto>(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription details for {SubscriptionId}", subscriptionId);
                throw;
            }
        }
        public async Task<int> ResetUserQuotasAsync()
        {
            var activeSubscriptions = await _unitOfWork.UserSubscriptionRepository
         .FindAsync(s => s.PaymentStatus == "Active", includeProperties: "Plan");

            int resetCount = 0;
            var now = DateTime.UtcNow;

            foreach (var sub in activeSubscriptions)
            {
                var plan = sub.Plan;
                if (plan == null || sub.LastQuotaResetDate?.Date == now.Date) continue;

                sub.CvRemaining = plan.MaxCvsAllowed;
                sub.InterviewPerDayRemaining = plan.MaxTextInterviewPerDay;
                sub.VoiceInterviewRemaining = plan.MaxVoiceInterviewPerMonth;
                sub.LastQuotaResetDate = now;

                resetCount++;
            }

            await _unitOfWork.SaveChanges();
            return resetCount;
        }

        public async Task<PremiumRateStatsDto> GetPremiumRateStatsAsync()
        {
            DateTime today = DateTime.UtcNow.Date;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Get all users
            var allUsers = await _unitOfWork.UserRepository.GetAllAsync(false);

            // Get all active premium subscriptions
            var activeSubs = await _unitOfWork.UserSubscriptionRepository.GetPremiumUserIds();

            var stats = new PremiumRateStatsDto
            {
                AllTime = CalculateRate(allUsers, activeSubs),
                Today = CalculateRate(allUsers.Where(u => u.CreatedAt.Value.Date == today), activeSubs),
                ThisWeek = CalculateRate(allUsers.Where(u => u.CreatedAt.Value.Date >= startOfWeek), activeSubs),
                ThisMonth = CalculateRate(allUsers.Where(u => u.CreatedAt.Value.Date >= startOfMonth), activeSubs),
            };

            return stats;
        }
        public int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;
        }
        private PremiumRateDetail CalculateRate(IEnumerable<User> users, List<int?> premiumUserIds)
        {
            var total = users.Count();
            var premium = users.Count(u => premiumUserIds.Contains(u.UserId));

            return new PremiumRateDetail
            {
                TotalUsers = total,
                PremiumUsers = premium,
                Rate = total == 0 ? 0 : Math.Round((decimal)premium / total * 100, 2)
            };
        }
        public async Task<SubscriptionQuotaDto?> GetUserQuotaAsync(int userId)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository.GetUserActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.Plan == null)
                return null;

            return new SubscriptionQuotaDto
            {
                InterviewPerDayRemaining = subscription.InterviewPerDayRemaining ?? 0,
                MaxInterviewPerDay = subscription.Plan.MaxTextInterviewPerDay ?? 0,
                VoiceInterviewRemaining = subscription.VoiceInterviewRemaining ?? 0,
                MaxVoiceInterview = subscription.Plan.MaxVoiceInterviewPerMonth ?? 0,
                CvRemaining = subscription.CvRemaining ?? 0,
                MaxCvsAllowed = subscription.Plan.MaxCvsAllowed ?? 0
            };
        }
    }
}
