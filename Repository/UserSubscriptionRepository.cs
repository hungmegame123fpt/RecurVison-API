using BusinessObject.Entities;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Interface;
using BusinessObject.DTO.UserSubscription;
using BusinessObject.DTO;

namespace Repository
{
    public class UserSubscriptionRepository : BaseRepository<UserSubscription>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(RecurVisionV1Context db) : base(db)
        {
        }

        public override async Task<UserSubscription> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(s => s.User)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.SubscriptionId == id);
        }
        public async Task<int> ExpireEndedSubscriptionsAsync()
        {
            var now = DateTime.UtcNow;

            var expiredSubscriptions = await _db.UserSubscriptions
                .Include(s => s.User)
                .Where(s => s.PaymentStatus == "ACTIVE" && s.EndDate != null && s.EndDate <= now)
                .ToListAsync();

            foreach (var sub in expiredSubscriptions)
            {
                sub.PaymentStatus = "EXPIRED";

                if (sub.User != null)
                {
                    sub.User.SubscriptionStatus = "EXPIRED";
                }
            }

            return await _db.SaveChangesAsync();
        }
        public async Task<UserSubscription?> GetUserActiveSubscriptionAsync(int? userId)
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.UserId == userId &&
                            s.PaymentStatus == "ACTIVE",
                includeProperties: "Plan,User"
            );
            return subscriptions.OrderByDescending(s => s.StartDate).FirstOrDefault();
        }

        public async Task<List<UserSubscription>> GetUserSubscriptionHistoryAsync(int userId)
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.UserId == userId,
                includeProperties: "Plan"
            );
            return subscriptions.OrderByDescending(s => s.StartDate).ToList();
        }

        public async Task<List<int?>> GetPremiumUserIds()
        {
            return await _db.UserSubscriptions
                .Where(s => s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<UserSubscription>> GetSubscriptionsByPlanAsync(int planId)
        {
            return await GetAllAsync(
                filter: s => s.PlanId == planId,
                includeProperties: "User,Plan"
            );
        }

        public async Task<List<UserSubscription>> GetExpiredSubscriptionsAsync()
        {
            return await GetAllAsync(
                filter: s => s.EndDate < DateTime.UtcNow && s.PaymentStatus == "ACTIVE",
                includeProperties: "User,Plan"
            );
        }
        public async Task<List<UserSubscription>> GetPaymentsInLast12MonthsAsync()
        {
            var fromDate = DateTime.UtcNow.AddMonths(-11).Date;
            return await _db.UserSubscriptions
                .Include(x => x.Plan)
                .Where(p => p.StartDate >= fromDate && p.PaymentStatus.Equals("ACTIVE") && p.PlanId != 15)
                .ToListAsync();
        }
        public async Task<List<UserSubscription>> GetExpiringSubscriptionsAsync(int daysFromNow)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysFromNow);
            return await GetAllAsync(
                filter: s => s.EndDate <= targetDate &&
                            s.EndDate > DateTime.UtcNow &&
                            s.PaymentStatus == "ACTIVE",
                includeProperties: "User,Plan"
            );
        }

        public async Task<List<UserSubscription>> GetAutoRenewSubscriptionsAsync()
        {
            return await GetAllAsync(
                filter: s => s.IsAutoRenew == true && s.PaymentStatus == "ACTIVE",
                includeProperties: "User,Plan"
            );
        }

        public async Task<UserSubscription?> GetSubscriptionByOrderCodeAsync(long orderCode)
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.PaymentStatus == $"PENDING:{orderCode}",
                includeProperties: "User,Plan"
            );
            return subscriptions.FirstOrDefault();
        }

        public async Task<bool> CancelSubscriptionAsync(int subscriptionId)
        {
            var subscription = await GetByIdAsync(subscriptionId);
            if (subscription == null || subscription.PaymentStatus != "ACTIVE")
                return false;

            subscription.PaymentStatus = "CANCELLED";
            subscription.IsAutoRenew = false;

            await UpdateAsync(subscription);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> RenewSubscriptionAsync(int subscriptionId, DateTime newEndDate)
        {
            var subscription = await GetByIdAsync(subscriptionId);
            if (subscription == null)
                return false;

            subscription.EndDate = newEndDate;
            subscription.LastPaymentDate = DateTime.UtcNow;
            subscription.PaymentStatus = "ACTIVE";

            await UpdateAsync(subscription);
            await SaveChangesAsync();
            return true;
        }

        public async Task<List<UserSubscription>> GetSubscriptionsByStatusAsync(string status)
        {
            return await GetAllAsync(
                filter: s => s.PaymentStatus.Contains(status),
                includeProperties: "User,Plan"
            );
        }

        public async Task<List<UserSubscription>> GetSubscriptionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await GetAllAsync(
                filter: s => s.StartDate >= startDate && s.StartDate <= endDate,
                includeProperties: "User,Plan"
            );
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.PaymentStatus == "ACTIVE" || s.PaymentStatus == "EXPIRED" || s.PaymentStatus == "CANCELLED",
                includeProperties: "Plan"
            );
            return subscriptions.Sum(s => s.Plan?.Price ?? 0);
        }

        public async Task<decimal> GetRevenueByPlanAsync(int planId)
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.PlanId == planId &&
                            (s.PaymentStatus == "ACTIVE" || s.PaymentStatus == "EXPIRED" || s.PaymentStatus == "CANCELLED"),
                includeProperties: "Plan"
            );
            return subscriptions.Sum(s => s.Plan?.Price ?? 0);
        }

        public async Task<int> GetActiveSubscriptionCountAsync()
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.PaymentStatus == "ACTIVE" && s.EndDate > DateTime.UtcNow
            );
            return subscriptions.Count;
        }

        public async Task<Dictionary<string, int>> GetSubscriptionStatsByStatusAsync()
        {
            var allSubscriptions = await GetAllAsync();
            return allSubscriptions
                .GroupBy(s => s.PaymentStatus ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<UserSubscription?> GetSubscriptionWithDetailsAsync(int subscriptionId)
        {
            var subscriptions = await GetAllAsync(
                filter: s => s.SubscriptionId == subscriptionId,
                includeProperties: "User,Plan"
            );
            return subscriptions.FirstOrDefault();
        }
        public async Task<List<TopCustomerDTO>> GetTopCustomersAsync()
        {
            var topCustomers = await _db.UserSubscriptions
             .Include(us => us.User)
             .Include(us => us.Plan)
             .Where(us => us.PaymentStatus == "ACTIVE" || us.PaymentStatus == "EXPIRED")
             .GroupBy(us => us.UserId)
             .Select(group => new TopCustomerDTO
             {
                 FullName = group.First().User.FirstName + " " + group.First().User.LastName,
                 Email = group.First().User.Email,
                 SubscriptionPlan = group.OrderByDescending(x => x.StartDate).First().Plan.PlanName,
                 TotalSpent = group.Sum(s => s.Plan.Price ?? 0),
                 LastPaymentDate = group.Max(s => s.LastPaymentDate) ?? DateTime.Now,
             })
         .Where(c => c.TotalSpent > 0)
         .OrderByDescending(c => c.TotalSpent)
         .Take(10)
         .ToListAsync();

            return topCustomers;
        }
    }
}
