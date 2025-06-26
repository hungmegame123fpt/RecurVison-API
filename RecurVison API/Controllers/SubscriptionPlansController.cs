using BusinessObject.DTO.SubscriptionPlan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.ComponentModel.DataAnnotations;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _planService;
        private readonly ILogger<SubscriptionPlansController> _logger;

        public SubscriptionPlansController(
            ISubscriptionPlanService planService,
            ILogger<SubscriptionPlansController> logger)
        {
            _planService = planService;
            _logger = logger;
        }

        /// <summary>
        /// Get all subscription plans
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetAllPlans()
        {
            try
            {
                var plans = await _planService.GetAllPlansAsync();
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription plans");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get active subscription plans only
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetActivePlans()
        {
            try
            {
                var plans = await _planService.GetActivePlansAsync();
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription plans");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plan by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanById(int id)
        {
            try
            {
                var plan = await _planService.GetPlanByIdAsync(id);
                if (plan == null)
                {
                    return NotFound($"Subscription plan with ID {id} not found");
                }
                return Ok(plan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plan by name
        /// </summary>
        [HttpGet("name/{planName}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanByName(string planName)
        {
            try
            {
                var plan = await _planService.GetPlanByNameAsync(planName);
                if (plan == null)
                {
                    return NotFound($"Subscription plan with name '{planName}' not found");
                }
                return Ok(plan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan by name {PlanName}", planName);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plans by user type
        /// </summary>
        [HttpGet("user-type/{userType}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetPlansByUserType(string userType)
        {
            try
            {
                var plans = await _planService.GetPlansByUserTypeAsync(userType);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans by user type {UserType}", userType);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plans by billing cycle
        /// </summary>
        [HttpGet("billing-cycle/{billingCycle}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetPlansByBillingCycle(string billingCycle)
        {
            try
            {
                var plans = await _planService.GetPlansByBillingCycleAsync(billingCycle);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans by billing cycle {BillingCycle}", billingCycle);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plans by price range
        /// </summary>
        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetPlansByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice)
        {
            try
            {
                var plans = await _planService.GetPlansByPriceRangeAsync(minPrice, maxPrice);
                return Ok(plans);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans by price range {MinPrice}-{MaxPrice}", minPrice, maxPrice);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Search subscription plans
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> SearchPlans([FromQuery] string searchTerm)
        {
            try
            {
                var plans = await _planService.SearchPlansAsync(searchTerm);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching subscription plans with term {SearchTerm}", searchTerm);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get filtered subscription plans
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetFilteredPlans([FromBody] PlanFilterDto filter)
        {
            try
            {
                var plans = await _planService.GetFilteredPlansAsync(filter);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered subscription plans");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubscriptionPlanDto>> CreatePlan([FromBody] CreateSubscriptionPlanDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var plan = await _planService.CreatePlanAsync(createDto);
                return CreatedAtAction(nameof(GetPlanById), new { id = plan.PlanId }, plan);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing subscription plan
        /// </summary>
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubscriptionPlanDto>> UpdatePlan(int id, [FromBody] UpdateSubscriptionPlanDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var plan = await _planService.UpdatePlanAsync(id, updateDto);
                return Ok(plan);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a subscription plan
        /// </summary>
        [HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletePlan(int id)
        {
            try
            {
                var result = await _planService.DeletePlanAsync(id);
                if (!result)
                {
                    return NotFound($"Subscription plan with ID {id} not found");
                }
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Activate a subscription plan
        /// </summary>
        [HttpPost("{id:int}/activate")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivatePlan(int id)
        {
            try
            {
                var result = await _planService.ActivatePlanAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating subscription plan");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id:int}/deactivate")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivatePlan(int id)
        {
            try
            {
                var result = await _planService.DeactivatePlanAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating subscription plan");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Get overall statistics of subscription plans.
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetSubscriptionStats()
        {
            try
            {
                var stats = await _planService.GetPlanStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving stats" });
            }
        }
    }
}
