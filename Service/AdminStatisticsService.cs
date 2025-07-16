using BusinessObject.Entities;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AdminStatisticsService : IAdminStatisticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminStatisticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> GetNewCvsTodayAsync() =>
        await _unitOfWork.CVRepository.CountAsync(c => c.UploadedAt.Value.Date == DateTime.UtcNow.Date);

        public async Task<int> GetNewInterviewsTodayAsync() =>
            await _unitOfWork.VirtualInterviewRepository.CountAsync(i => i.CreatedAt.Value.Date == DateTime.UtcNow.Date);

        public async Task<int> GetNewUsersTodayAsync() =>
            await _unitOfWork.UserRepository.CountAsync(u => u.CreatedAt.Value.Date == DateTime.UtcNow.Date);

        public async Task<int> GetTotalCvsAsync() =>
            await _unitOfWork.CVRepository.CountAsync();

        public async Task<object> GetTotalCvFieldsAsync() => new
        {
            Skills = await _unitOfWork.CvSkill.CountAsync(),
            Projects = await _unitOfWork.CvProject.CountAsync(),
            Educations = await _unitOfWork.CvEducation.CountAsync(),
            Certifications = await _unitOfWork.CvCertification.CountAsync()
        };

        public async Task<int> GetTotalInterviewsAsync() =>
            await _unitOfWork.VirtualInterviewRepository.CountAsync();

        public async Task<int> GetInterviewsInProgressAsync() =>
            await _unitOfWork.VirtualInterviewRepository.CountAsync(i => i.Status == "InProgress");

        //public async Task<List<JobField>> GetTopLikedJobsAsync(int top = 5) =>
        //    (await _jobRepo.GetAllAsync())
        //        .OrderByDescending(j => j.LikeCount)
        //        .Take(top)
        //        .ToList();

        //public async Task<List<Course>> GetTopLikedCoursesAsync(int top = 5) =>
        //    (await _courseRepo.GetAllAsync())
        //        .OrderByDescending(c => c.LikeCount)
        //        .Take(top)
        //        .ToList();



        public async Task<object> GetCvFieldDistributionAsync() => await GetTotalCvFieldsAsync();

        public async Task<List<object>> GetCvAnalysisOverTimeAsync()
        {
            var results = await _unitOfWork.CvAnalysisResult.GetAllAsync();
            return results.GroupBy(r => r.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .Cast<object>()
                .ToList();
        }

        public async Task<List<object>> GetInterviewCountOverTimeAsync()
        {
            var interviews = await _unitOfWork.VirtualInterviewRepository.GetAllAsync();
            return interviews.GroupBy(i => i.CreatedAt.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .Cast<object>()
                .ToList();
        }

        public async Task<List<object>> GetScoreDistributionAsync()
        {
            var results = await _unitOfWork.CvAnalysisResult.GetAllAsync();
            return results.GroupBy(r => r.MatchScore)
                .Select(g => new { Score = g.Key, Count = g.Count() })
                .Cast<object>()
                .ToList();
        }
        public async Task<object> GetDashboardOverviewAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(includeProperties: "Cvs");
            var userIds = users.Select(u => u.UserId).ToList();

            var analysisList = await _unitOfWork.CvAnalysisResult.GetAllAsync(includeProperties: "Cv");
            var interviewList = await _unitOfWork.VirtualInterviewRepository.GetAllAsync();
            var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetAllAsync(includeProperties: "Plan");

            // User Engagement
            var aiUsers = interviewList.Select(i => i.UserId).Distinct().ToHashSet();
            var cvUsers = analysisList.Select(a => a.Cv.UserId).Distinct().ToHashSet();

            var both = aiUsers.Intersect(cvUsers).ToHashSet();
            var aiOnly = aiUsers.Except(cvUsers).ToHashSet();
            var cvOnly = cvUsers.Except(aiUsers).ToHashSet();
            var noUse = userIds.Except(aiUsers.Union(cvUsers)).ToHashSet();
            var totalUsers = users.Count;

            var userEngagement = new List<object>
        {
            new { label = "AI Interview", count = aiOnly.Count, percentage = Math.Round(aiOnly.Count * 100.0 / totalUsers, 1) },
            new { label = "CV Analysis", count = cvOnly.Count, percentage = Math.Round(cvOnly.Count * 100.0 / totalUsers, 1) },
            new { label = "Both Services", count = both.Count, percentage = Math.Round(both.Count * 100.0 / totalUsers, 1) },
            new { label = "Not Participated", count = noUse.Count, percentage = Math.Round(noUse.Count * 100.0 / totalUsers, 1) }
        };
            return userEngagement;
        }
        public async Task<object> GetPackagePurchaseStatsAsync()
        {
            var allPlans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();

            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var allSubs = await _unitOfWork.UserSubscriptionRepository.GetAllAsync(includeProperties: "Plan");

            var stats = allPlans.Select(plan =>
            {
                var planSubs = allSubs.Where(sub => sub.PlanId == plan.PlanId);

                return new
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    DailyCount = planSubs.Count(s => s.StartDate.HasValue && s.StartDate.Value.Date == today),
                    WeeklyCount = planSubs.Count(s => s.StartDate.HasValue && s.StartDate.Value.Date >= weekStart),
                    MonthlyCount = planSubs.Count(s => s.StartDate.HasValue && s.StartDate.Value.Date >= monthStart)
                };
            }).ToList();

            return stats;
        }
    }
}
