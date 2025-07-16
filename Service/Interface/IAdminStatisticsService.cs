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
        Task<int> GetNewCvsTodayAsync();
        Task<int> GetNewInterviewsTodayAsync();
        Task<int> GetNewUsersTodayAsync();
        Task<int> GetTotalCvsAsync();
        Task<object> GetTotalCvFieldsAsync();
        Task<int> GetTotalInterviewsAsync();
        Task<int> GetInterviewsInProgressAsync();
        //Task<List<JobField>> GetTopLikedJobsAsync(int top = 5);
        //Task<List<Course>> GetTopLikedCoursesAsync(int top = 5);
        //Task<List<object>> GetSubscriptionsOverTimeAsync();
        //Task<List<object>> GetPaidRatioOverTimeAsync();
        Task<object> GetCvFieldDistributionAsync();
        Task<List<object>> GetCvAnalysisOverTimeAsync();
        Task<List<object>> GetInterviewCountOverTimeAsync();
        Task<List<object>> GetScoreDistributionAsync();
        Task<object> GetDashboardOverviewAsync();
    }
}
