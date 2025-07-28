using BusinessObject.DTO;
using BusinessObject.DTO.UserSubscription;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserSubscriptionRepository : IBaseRepository<UserSubscription>
    {
        Task<List<int?>> GetPremiumUserIds();
        Task<int> ExpireEndedSubscriptionsAsync();
        Task<UserSubscription?> GetUserActiveSubscriptionAsync(int? userId);
        Task<List<UserSubscription>> GetUserSubscriptionHistoryAsync(int userId);
        Task<List<UserSubscription>> GetSubscriptionsByPlanAsync(int planId);
        Task<List<UserSubscription>> GetExpiredSubscriptionsAsync();
        Task<List<UserSubscription>> GetExpiringSubscriptionsAsync(int daysFromNow);
        Task<List<UserSubscription>> GetAutoRenewSubscriptionsAsync();
        Task<UserSubscription?> GetSubscriptionByOrderCodeAsync(long orderCode);
        Task<bool> CancelSubscriptionAsync(int subscriptionId);
        Task<bool> RenewSubscriptionAsync(int subscriptionId, DateTime newEndDate);
        Task<List<UserSubscription>> GetSubscriptionsByStatusAsync(string status);
        Task<List<UserSubscription>> GetSubscriptionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByPlanAsync(int planId);
        Task<int> GetActiveSubscriptionCountAsync();
        Task<List<UserSubscription>> GetPaymentsInLast12MonthsAsync();
        Task<Dictionary<string, int>> GetSubscriptionStatsByStatusAsync();
        Task<UserSubscription?> GetSubscriptionWithDetailsAsync(int subscriptionId);
        Task<List<TopCustomerDTO>> GetTopCustomersAsync();
    }
}
