using BusinessObject.DTO.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Repository.Interface;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionPaymentService _subscriptionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOS _payOS;

        public SubscriptionController(
            ISubscriptionPaymentService subscriptionService,
            IUnitOfWork unitOfWork,
            PayOS payOS)
        {
            _subscriptionService = subscriptionService;
            _unitOfWork = unitOfWork;
            _payOS = payOS;
        }
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreateSubscriptionPayment([FromBody] CreatePaymentLinkRequest request)
        {
            var result = await _subscriptionService.CreateSubscriptionPaymentAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] WebhookType body)
        {
            try
            {
                // Verify webhook data
                WebhookData data = _payOS.verifyPaymentWebhookData(body);

                var webhookRequest = new PaymentWebhookRequest
                {
                    OrderCode = data.orderCode,
                    Status = data.code == "00" ? "PAID" : "CANCELLED",
                    Amount = data.amount,
                    Description = data.description
                };

                var processed = await _subscriptionService.ProcessPaymentWebhookAsync(webhookRequest);

                return Ok(new { success = processed });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }
        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetUserActiveSubscription(int userId)
        {
            var subscription = await _subscriptionService.GetUserActiveSubscriptionAsync(userId);

            if (subscription == null)
            {
                return NotFound("No active subscription found");
            }

            return Ok(subscription);
        }

        [HttpGet("user/{userId}/history")]
        public async Task<IActionResult> GetUserSubscriptionHistory(int userId)
        {
            var subscriptions = await _subscriptionService.GetUserActiveSubscriptionAsync(userId);
            return Ok(subscriptions);
        }

        [HttpPost("{subscriptionId}/cancel")]
        public async Task<IActionResult> CancelSubscription(int subscriptionId)
        {
            var result = await _subscriptionService.CancelSubscriptionAsync(subscriptionId);

            if (result)
            {
                return Ok(new { message = "Subscription cancelled successfully" });
            }

            return BadRequest(new { message = "Failed to cancel subscription" });
        }

        [HttpPost("{subscriptionId}/renew")]
        public async Task<IActionResult> RenewSubscription(int subscriptionId)
        {
            var result = await _subscriptionService.RenewSubscriptionAsync(subscriptionId);

            if (result)
            {
                return Ok(new { message = "Subscription renewed successfully" });
            }

            return BadRequest(new { message = "Failed to renew subscription" });
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetActiveSubscriptionPlans()
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetActivePlansAsync();
            return Ok(plans);
        }

        [HttpGet("plans/{planId}")]
        public async Task<IActionResult> GetSubscriptionPlan(int planId)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(planId);

            if (plan == null)
            {
                return NotFound("Subscription plan not found");
            }

            return Ok(plan);
        }

        [HttpGet("plans/search")]
        public async Task<IActionResult> SearchSubscriptionPlans([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            var plans = await _unitOfWork.SubscriptionPlanRepository.SearchPlansAsync(searchTerm);
            return Ok(plans);
        }

        [HttpGet("expiring")]
        public async Task<IActionResult> GetExpiringSubscriptions([FromQuery] int days = 7)
        {
            var subscriptions = await _subscriptionService.GetExpiringSubscriptionsAsync(days);
            return Ok(subscriptions);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetSubscriptionStatistics()
        {
            var stats = await _subscriptionService.GetSubscriptionStatsAsync();
            var totalRevenue = await _subscriptionService.GetTotalRevenueAsync();
            var activeCount = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionCountAsync();

            return Ok(new
            {
                StatusStats = stats,
                TotalRevenue = totalRevenue,
                ActiveSubscriptions = activeCount
            });
        }

        [HttpGet("revenue/total")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var revenue = await _subscriptionService.GetTotalRevenueAsync();
            return Ok(new { totalRevenue = revenue });
        }

        [HttpGet("revenue/plan/{planId}")]
        public async Task<IActionResult> GetRevenueByPlan(int planId)
        {
            var revenue = await _unitOfWork.UserSubscriptionRepository.GetRevenueByPlanAsync(planId);
            return Ok(new { planRevenue = revenue });
        }
    }
}
