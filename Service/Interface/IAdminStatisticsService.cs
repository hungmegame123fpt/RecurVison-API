using BusinessObject.DTO;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAdminStatisticsService
    {
        Task<StatsComparisonDto> GetNewCvsStatsAsync();
        Task<StatsComparisonDto> GetNewInterviewsStatsAsync();
        Task<StatsComparisonDto> GetNewCompletedInterviewsStatsAsync();
        Task<StatsComparisonDto> GetNewUsersStatsAsync();
        Task<int> GetTotalCvsAsync();
        Task<object> GetTotalCvFieldsAsync();
        Task<int> GetTotalInterviewsAsync();
        Task<int> GetInterviewsInProgressAsync();
        Task<RevenueAnalyticsResponse> GetAnalyticsAsync();
        Task<List<TopCustomerDTO>> GetTopCustomersAsync();
        //Task<List<JobField>> GetTopLikedJobsAsync(int top = 5);
        //Task<List<Course>> GetTopLikedCoursesAsync(int top = 5);
        //Task<List<object>> GetSubscriptionsOverTimeAsync();
        //Task<List<object>> GetPaidRatioOverTimeAsync();
        Task<object> GetCvFieldDistributionAsync();
        Task<List<object>> GetCvAnalysisOverTimeAsync();
        Task<List<object>> GetInterviewCountOverTimeAsync();
        Task<List<object>> GetScoreDistributionAsync();
        Task<object> GetDashboardOverviewAsync();
        Task<object> GetPackagePurchaseStatsAsync();
        Task<List<UserListDto>> GetUserListAsync();
        Task<TimeSeriesAnalysisDto> GetCvAnalysesAsync(DateTime from, DateTime to, string range);
        Task<TimeSeriesAnalysisDto> GetInterviewSessionsAsync(DateTime from, DateTime to, string range);
        Task<PremiumConversionRateDto> GetPremiumConversionRateAsync();
        Task<List<ScoreHistogramBinDto>> GetUserScoreHistogramAsync();

    }
}
