using BusinessObject.DTO.UserSubscription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSubscriptionController : ControllerBase
    {
        private readonly IUserSubscriptionService _service;
        private readonly ILogger<UserSubscriptionController> _logger;

        public UserSubscriptionController(
            IUserSubscriptionService service,
            ILogger<UserSubscriptionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<UserSubscriptionDto>> GetUserActiveSubscription(int userId)
        {
            try
            {
                var subscription = await _service.GetUserActiveSubscriptionAsync(userId);
                if (subscription == null)
                    return NotFound($"No active subscription found for user {userId}");

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}/history")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetUserSubscriptionHistory(int userId)
        {
            try
            {
                var subscriptions = await _service.GetUserSubscriptionHistoryAsync(userId);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription history for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("plan/{planId}")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetSubscriptionsByPlan(int planId)
        {
            try
            {
                var subscriptions = await _service.GetSubscriptionsByPlanAsync(planId);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for plan {PlanId}", planId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("expired")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetExpiredSubscriptions()
        {
            try
            {
                var subscriptions = await _service.GetExpiredSubscriptionsAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expired subscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("expiring/{daysFromNow}")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetExpiringSubscriptions(int daysFromNow)
        {
            try
            {
                if (daysFromNow < 0)
                    return BadRequest("Days from now must be a positive number");

                var subscriptions = await _service.GetExpiringSubscriptionsAsync(daysFromNow);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring subscriptions within {Days} days", daysFromNow);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("auto-renew")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetAutoRenewSubscriptions()
        {
            try
            {
                var subscriptions = await _service.GetAutoRenewSubscriptionsAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting auto-renew subscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("order/{orderCode}")]
        public async Task<ActionResult<UserSubscriptionDto>> GetSubscriptionByOrderCode(long orderCode)
        {
            try
            {
                var subscription = await _service.GetSubscriptionByOrderCodeAsync(orderCode);
                if (subscription == null)
                    return NotFound($"No subscription found with order code {orderCode}");

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription by order code {OrderCode}", orderCode);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{subscriptionId}/cancel")]
        public async Task<ActionResult> CancelSubscription(int subscriptionId)
        {
            try
            {
                var result = await _service.CancelSubscriptionAsync(subscriptionId);
                if (!result)
                    return BadRequest($"Unable to cancel subscription {subscriptionId}");

                return Ok(new { message = "Subscription cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling subscription {SubscriptionId}", subscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{subscriptionId}/renew")]
        public async Task<ActionResult> RenewSubscription(int subscriptionId, [FromBody] RenewSubscriptionRequest request)
        {
            try
            {
                if (request.NewEndDate <= DateTime.UtcNow)
                    return BadRequest("New end date must be in the future");

                var result = await _service.RenewSubscriptionAsync(subscriptionId, request.NewEndDate);
                if (!result)
                    return BadRequest($"Unable to renew subscription {subscriptionId}");

                return Ok(new { message = "Subscription renewed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing subscription {SubscriptionId}", subscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetSubscriptionsByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest("Status cannot be empty");

                var subscriptions = await _service.GetSubscriptionsByStatusAsync(status);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions by status {Status}", status);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<List<UserSubscriptionDto>>> GetSubscriptionsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest("Start date cannot be after end date");

                var subscriptions = await _service.GetSubscriptionsByDateRangeAsync(startDate, endDate);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions by date range {StartDate} - {EndDate}", startDate, endDate);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("revenue/total")]
        public async Task<ActionResult<decimal>> GetTotalRevenue()
        {
            try
            {
                var revenue = await _service.GetTotalRevenueAsync();
                return Ok(new { totalRevenue = revenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("revenue/plan/{planId}")]
        public async Task<ActionResult<decimal>> GetRevenueByPlan(int planId)
        {
            try
            {
                var revenue = await _service.GetRevenueByPlanAsync(planId);
                return Ok(new { planId, revenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue for plan {PlanId}", planId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("count/active")]
        public async Task<ActionResult<int>> GetActiveSubscriptionCount()
        {
            try
            {
                var count = await _service.GetActiveSubscriptionCountAsync();
                return Ok(new { activeSubscriptionCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription count");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("stats/status")]
        public async Task<ActionResult<Dictionary<string, int>>> GetSubscriptionStatsByStatus()
        {
            try
            {
                var stats = await _service.GetSubscriptionStatsByStatusAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription stats by status");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{subscriptionId}/details")]
        public async Task<ActionResult<UserSubscriptionDto>> GetSubscriptionWithDetails(int subscriptionId)
        {
            try
            {
                var subscription = await _service.GetSubscriptionWithDetailsAsync(subscriptionId);
                if (subscription == null)
                    return NotFound($"Subscription {subscriptionId} not found");

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription details for {SubscriptionId}", subscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
