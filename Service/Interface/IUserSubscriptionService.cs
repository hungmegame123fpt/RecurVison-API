using BusinessObject.DTO.UserSubscription;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IUserSubscriptionService
    {
        Task<int> ExpireEndedSubscriptionsAsync();
        Task<UserSubscriptionDto> UpdateSubscriptionAsync(int id, UserSubscriptionDto subscriptionDto);
        Task<UserSubscriptionDto?> GetUserActiveSubscriptionAsync(int userId);
        Task<List<UserSubscriptionDto>> GetUserSubscriptionHistoryAsync(int userId);
        Task<List<UserSubscriptionDto>> GetSubscriptionsByPlanAsync(int planId);
        Task<List<UserSubscriptionDto>> GetExpiredSubscriptionsAsync();
        Task<List<UserSubscriptionDto>> GetExpiringSubscriptionsAsync(int daysFromNow);
        Task<List<UserSubscriptionDto>> GetAutoRenewSubscriptionsAsync();
        Task<UserSubscriptionDto?> GetSubscriptionByOrderCodeAsync(long orderCode);
        Task<bool> CancelSubscriptionAsync(int subscriptionId);
        Task<bool> RenewSubscriptionAsync(int subscriptionId, DateTime newEndDate);
        Task<List<UserSubscriptionDto>> GetSubscriptionsByStatusAsync(string status);
        Task<List<UserSubscriptionDto>> GetSubscriptionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByPlanAsync(int planId);
        Task<int> GetActiveSubscriptionCountAsync();
        Task<Dictionary<string, int>> GetSubscriptionStatsByStatusAsync();
        Task<UserSubscriptionDto?> GetSubscriptionWithDetailsAsync(int subscriptionId);
        Task<PremiumRateStatsDto> GetPremiumRateStatsAsync();
        Task<int> ResetUserQuotasAsync();
        Task<SubscriptionQuotaDto?> GetUserQuotaAsync(int userId);
    }
}
