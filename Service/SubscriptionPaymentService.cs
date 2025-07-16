using BusinessObject.DTO.Payment;
using BusinessObject.Entities;
using Google;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Interface;
using Repository.Interface;
using Repository;

namespace Service
{
    public class SubscriptionPaymentService : ISubscriptionPaymentService
    {
        private readonly PayOS _payOS;
        private readonly IUnitOfWork _unitOfWork;    
        private readonly ILogger<SubscriptionPaymentService> _logger;

        public SubscriptionPaymentService(
            PayOS payOS,
            IUnitOfWork unitOfWork,
            ILogger<SubscriptionPaymentService> logger)
        {
            _payOS = payOS;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response> CreateSubscriptionPaymentAsync(CreatePaymentLinkRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            var userSubscriptions = await _unitOfWork.UserSubscriptionRepository.GetUserSubscriptionHistoryAsync(request.UserId);
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(request.PlanId);
            if (plan == null || plan.IsActive != true)
            {
                await _unitOfWork.RollbackAsync();
                return new Response
                {
                    Success = false,
                    Message = "Subscription plan not found or inactive"
                };
            }
            // Check if user already has active subscription using repository
            var existingSubscription = await _unitOfWork.UserSubscriptionRepository.GetUserActiveSubscriptionAsync(request.UserId);
            if (existingSubscription != null && existingSubscription.PlanId != 15)
            {
                await _unitOfWork.RollbackAsync();
                return new Response
                {
                    Success = false,
                    Message = "User already has an active subscription"
                };
            }          
            // Create pending subscription record using repository
            var subscription = new UserSubscription
            {
                UserId = request.UserId,
                PlanId = request.PlanId,
                StartDate = null, // Will be set when payment is confirmed
                EndDate = null,
                IsAutoRenew = false,
                PaymentStatus = "PENDING",
                LastPaymentDate = null,
                CvRemaining = plan.MaxCvsAllowed,
                InterviewPerDayRemaining = plan.MaxTextInterviewPerDay,
                VoiceInterviewRemaining = plan.MaxVoiceInterviewPerMonth,
                LastQuotaResetDate = DateTime.Now,
            };

            await _unitOfWork.UserSubscriptionRepository.CreateAsync(subscription);
            await _unitOfWork.SaveChanges();

            // Generate unique order code
            long orderCode = DateTimeOffset.Now.ToUnixTimeSeconds();

            // Create PayOS payment request
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)(plan.Price ?? 0), // PayOS expects amount in VND as integer
                description: request.Description ?? $"Subscription payment for {plan.PlanName}",
                items: new List<ItemData>
                {
                    new ItemData(
                        name: plan.PlanName,
                        quantity: 1,
                        price: (int)(plan.Price ?? 0)
                    )
                },
                returnUrl: request.ReturnUrl,
                cancelUrl: request.CancelUrl
            );

            // Create payment link
            var createPayment = await _payOS.createPaymentLink(paymentData);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            // Store order code for webhook processing using repository
            subscription.PaymentStatus = $"PENDING:{orderCode}";
            user.SubscriptionStatus = "PENDING";
            await _unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChanges();

            // Commit transaction
            await _unitOfWork.CommitAsync();

            return new Response
            {
                Success = true,
                Message = "Payment link created successfully",
                PaymentUrl = createPayment.checkoutUrl,
                OrderCode = orderCode,
                SubscriptionId = subscription.SubscriptionId
            };
        }

        public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookRequest webhookRequest)
        {
        await _unitOfWork.BeginTransactionAsync();

        // Find subscription by order code using repository
        var subscription = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionByOrderCodeAsync(webhookRequest.OrderCode);
        var user = await _unitOfWork.UserRepository.GetByIdAsync(subscription.UserId);
            if (subscription == null)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogWarning("Subscription not found for order code: {OrderCode}", webhookRequest.OrderCode);
            return false;
        }

        if (webhookRequest.Status.ToUpper() == "PAID")
        {
            // Calculate subscription period
            var startDate = DateTime.UtcNow;
            var endDate = CalculateEndDate(startDate, subscription.Plan?.BillingCycle);
            // Update subscription using repository
            subscription.PaymentStatus = "ACTIVE";
            subscription.StartDate = startDate;
            subscription.EndDate = endDate;
            subscription.LastPaymentDate = startDate;
            await _unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
            await _unitOfWork.SaveChanges();
            await _unitOfWork.CommitAsync();
            //Update User Subscriptipn status
            user.SubscriptionStatus = "ACTIVE";
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChanges();
            await _unitOfWork.CommitAsync();         
            _logger.LogInformation("Subscription {SubscriptionId} activated for user {UserId}",
            subscription.SubscriptionId, subscription.UserId);

            return true;
        }
        else if (webhookRequest.Status.ToUpper() == "CANCELLED")
        {
            subscription.PaymentStatus = "CANCELLED";
            await _unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
            await _unitOfWork.SaveChanges();
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Subscription payment cancelled for order code: {OrderCode}", webhookRequest.OrderCode);
            return true;
        }

        await _unitOfWork.RollbackAsync();
        return false;
    }

        public async Task<UserSubscription?> GetUserActiveSubscriptionAsync(int userId)
        {
            return await _unitOfWork.UserSubscriptionRepository.GetUserActiveSubscriptionAsync(userId);
        }

        public async Task<bool> CancelSubscriptionAsync(int subscriptionId)
        {
              var result =  await _unitOfWork.UserSubscriptionRepository.CancelSubscriptionAsync(subscriptionId);
                return result;
        }      
        public async Task<bool> RenewSubscriptionAsync(int subscriptionId)
        {
            await _unitOfWork.BeginTransactionAsync();

            var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(subscriptionId);
            if (subscription?.Plan == null)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            // Calculate new end date
            var currentEndDate = subscription.EndDate ?? DateTime.UtcNow;
            var newEndDate = CalculateEndDate(currentEndDate, subscription.Plan.BillingCycle);

            // Renew using repository method
            var result = await _unitOfWork.UserSubscriptionRepository.RenewSubscriptionAsync(subscriptionId, newEndDate);

            if (result)
            {
                await _unitOfWork.CommitAsync();
            }
            else
            {
                await _unitOfWork.RollbackAsync();
            }

            return result;
        }
        public async Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysFromNow)
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.GetExpiringSubscriptionsAsync(daysFromNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring subscriptions for {Days} days", daysFromNow);
                return new List<UserSubscription>();
            }
        }

        public async Task<Dictionary<string, int>> GetSubscriptionStatsAsync()
        {
            try
            {
                return await _unitOfWork.UserSubscriptionRepository.GetSubscriptionStatsByStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription statistics");
                return new Dictionary<string, int>();
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
                _logger.LogError(ex, "Error calculating total revenue");
                return 0;
            }
        }
        private DateTime CalculateEndDate(DateTime startDate, string? billingCycle)
        {
            return billingCycle?.ToLower() switch
            {
                "monthly" => startDate.AddMonths(1),
                "quarterly" => startDate.AddMonths(3),
                "yearly" => startDate.AddYears(1),
                "weekly" => startDate.AddDays(7),
                _ => startDate.AddMonths(1) // Default to monthly
            };
        }
    }
}
