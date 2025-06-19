using BusinessObject.DTO.Payment;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ISubscriptionPaymentService
    {
        Task<Response> CreateSubscriptionPaymentAsync(CreatePaymentLinkRequest request);
        Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookRequest webhookRequest);
        Task<UserSubscription?> GetUserActiveSubscriptionAsync(int userId);
        Task<bool> CancelSubscriptionAsync(int subscriptionId);
        Task<bool> RenewSubscriptionAsync(int subscriptionId);
        Task<decimal> GetTotalRevenueAsync();
        Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysFromNow);
        Task<Dictionary<string, int>> GetSubscriptionStatsAsync();
    }
}
