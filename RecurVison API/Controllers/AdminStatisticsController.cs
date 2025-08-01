﻿using BusinessObject.DTO;
using BusinessObject.DTO.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using Service;
using Service.Interface;

namespace RecurVison_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStatisticsController : ControllerBase
    {
        private readonly IAdminStatisticsService _statisticsService;
        private readonly IUserService _userService;
        private readonly ISubscriptionPaymentService _subscriptionService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminStatisticsController(IAdminStatisticsService statisticsService, IUserService userService, ISubscriptionPaymentService subscriptionService, IUnitOfWork unitOfWork)
        {
            _statisticsService = statisticsService;
            _userService = userService;
            _subscriptionService = subscriptionService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("cvs/new-today")]
        public async Task<IActionResult> GetNewCvsToday()
        {
            var count = await _statisticsService.GetNewCvsStatsAsync();
            return Ok(count);
        }

        [HttpGet("interviews/new-today")]
        public async Task<IActionResult> GetNewInterviewsToday()
        {
            var count = await _statisticsService.GetNewInterviewsStatsAsync();
            return Ok(count);
        }
        [HttpGet("interviews/completed-today")]
        public async Task<IActionResult> GetCompletedInterviewsToday()
        {
            var count = await _statisticsService.GetNewCompletedInterviewsStatsAsync();
            return Ok(count);
        }

        [HttpGet("users/new-today")]
        public async Task<IActionResult> GetNewUsersToday()
        {
            var count = await _statisticsService.GetNewUsersStatsAsync();
            return Ok(count);
        }

        [HttpGet("cvs/total")]
        public async Task<IActionResult> GetTotalCvs()
        {
            var total = await _statisticsService.GetTotalCvsAsync();
            return Ok(total);
        }

        [HttpGet("cv-fields/total")]
        public async Task<IActionResult> GetTotalCvFields()
        {
            var fields = await _statisticsService.GetTotalCvFieldsAsync();
            return Ok(fields);
        }

        [HttpGet("interviews/total")]
        public async Task<IActionResult> GetTotalInterviews()
        {
            var total = await _statisticsService.GetTotalInterviewsAsync();
            return Ok(total);
        }

        [HttpGet("interviews/in-progress")]
        public async Task<IActionResult> GetInterviewsInProgress()
        {
            var count = await _statisticsService.GetInterviewsInProgressAsync();
            return Ok(count);
        }

        [HttpGet("cv-fields/distribution")]
        public async Task<IActionResult> GetCvFieldDistribution()
        {
            var result = await _statisticsService.GetCvFieldDistributionAsync();
            return Ok(result);
        }

        [HttpGet("cv-analysis/over-time")]
        public async Task<IActionResult> GetCvAnalysisOverTime()
        {
            var result = await _statisticsService.GetCvAnalysisOverTimeAsync();
            return Ok(result);
        }

        [HttpGet("interviews/over-time")]
        public async Task<IActionResult> GetInterviewCountOverTime()
        {
            var result = await _statisticsService.GetInterviewCountOverTimeAsync();
            return Ok(result);
        }

        [HttpGet("analysis/score-distribution")]
        public async Task<IActionResult> GetScoreDistribution()
        {
            var result = await _statisticsService.GetScoreDistributionAsync();
            return Ok(result);
        }
        [HttpGet("user/stats")]
        public async Task<ActionResult<UserStatsDto>> GetUserStats()
        {
            try
            {
                var stats = await _userService.GetUserStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("subscription/stats")]
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
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueAnalytics()
        {
            var data = await _statisticsService.GetAnalyticsAsync();
            return Ok(data);
        }
        [HttpGet("revenue/plan/{planId}")]
        public async Task<IActionResult> GetRevenueByPlan(int planId)
        {
            var revenue = await _unitOfWork.UserSubscriptionRepository.GetRevenueByPlanAsync(planId);
            return Ok(new { planRevenue = revenue });
        }
        [HttpGet("user-engagement")]
        public async Task<IActionResult> GetDashboardOverview()
        {
            var result = await _statisticsService.GetDashboardOverviewAsync();
            return Ok(result);
        }
        [HttpGet("package-purchases")]
        public async Task<IActionResult> GetPackagePurchaseStats()
        {
            var stats = await _statisticsService.GetPackagePurchaseStatsAsync();
            return Ok(stats);
        }
        [HttpGet("admin/userList")]
        public async Task<IActionResult> GetUserList()
        {
            var users = await _statisticsService.GetUserListAsync();
            return Ok(users);
        }
        [HttpGet("interview-sessions")]
        public async Task<IActionResult> GetInterviewSessions([FromQuery] string range = "daily")
        {
            var (from, to) = GetDateRange(range);
            var result = await _statisticsService.GetInterviewSessionsAsync(from, to, range);
            return Ok(result);
        }

        [HttpGet("cv-analysis")]
        public async Task<IActionResult> GetCvAnalyses([FromQuery] string range = "daily")
        {
            var (from, to) = GetDateRange(range);
            var result = await _statisticsService.GetCvAnalysesAsync(from, to, range);
            return Ok(result);
        }
        [HttpGet("premium-conversion-rate")]
        public async Task<IActionResult> GetPremiumConversionRate()
        {
            var result = await _statisticsService.GetPremiumConversionRateAsync();
            return Ok(result);
        }
        [HttpGet("interviews/user-score-histogram")]
        public async Task<IActionResult> GetScoreHistogram()
        {
            var result = await _statisticsService.GetUserScoreHistogramAsync();
            return Ok(result);
        }
        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers()
        {
            var result = await _statisticsService.GetTopCustomersAsync();
            return Ok(result);
        }
        [HttpGet("monthly-highlights")]
        public async Task<IActionResult> GetMonthlyJobHighlights()
        {
            var result = await _statisticsService.GetMonthlyJobHighlightsAsync();
            return Ok(new APIResponse<MonthlyJobHighlightDto>
            {
                Success = true,
                Message = "Monthly job highlights fetched successfully.",
                Data = result
            });
        }
        private (DateTime from, DateTime to) GetDateRange(string range)
        {
            var to = DateTime.UtcNow.Date;
            DateTime from;

            switch (range.ToLower())
            {
                case "weekly":
                    from = to.AddDays(-7 * 6); // last 7 weeks (inclusive)
                    break;
                case "monthly":
                    from = new DateTime(to.Year, to.Month, 1).AddMonths(-6); // last 7 months
                    break;
                default:
                    from = to.AddDays(-6); // last 7 days
                    break;
            }

            return (from, to);
        }
        
    }
}
