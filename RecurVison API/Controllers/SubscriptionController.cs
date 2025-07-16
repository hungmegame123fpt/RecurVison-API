using BusinessObject.DTO.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Repository.Interface;
using Service;
using Service.Interface;
using System.Security.Claims;

namespace RecurVison_API.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

           request.UserId = int.Parse(userId);
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
        [HttpGet("user/active")]
        public async Task<IActionResult> GetUserActiveSubscription()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
            var subscription = await _subscriptionService.GetUserActiveSubscriptionAsync(userId);

            if (subscription == null)
            {
                return NotFound("No active subscription found");
            }

            return Ok(subscription);
        }

        [HttpGet("user/history")]
        public async Task<IActionResult> GetUserSubscriptionHistory()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in claims.");
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
    }
}
