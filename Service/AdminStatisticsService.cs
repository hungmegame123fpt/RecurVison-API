using BusinessObject.DTO;
using BusinessObject.Entities;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public async Task<StatsComparisonDto> GetNewCvsStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var todayCount = await _unitOfWork.CVRepository.CountAsync(c => c.UploadedAt.HasValue && c.UploadedAt.Value.Date == today);
            var yesterdayCount = await _unitOfWork.CVRepository.CountAsync(c => c.UploadedAt.HasValue && c.UploadedAt.Value.Date == yesterday);

            return new StatsComparisonDto
            {
                Today = todayCount,
                Yesterday = yesterdayCount,
                PercentageChange = CalculatePercentageChange(todayCount, yesterdayCount)
            };
        }

        public async Task<StatsComparisonDto> GetNewInterviewsStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var todayCount = await _unitOfWork.VirtualInterviewRepository.CountCompletedInterviewsAsync(today);
            var yesterdayCount = await _unitOfWork.VirtualInterviewRepository.CountCompletedInterviewsAsync(yesterday);

            return new StatsComparisonDto
            {
                Today = todayCount,
                Yesterday = yesterdayCount,
                PercentageChange = CalculatePercentageChange(todayCount, yesterdayCount)
            };
        }

        public async Task<StatsComparisonDto> GetNewUsersStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var todayCount = await _unitOfWork.UserRepository.CountAsync(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date == today);
            var yesterdayCount = await _unitOfWork.UserRepository.CountAsync(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date == yesterday);

            return new StatsComparisonDto
            {
                Today = todayCount,
                Yesterday = yesterdayCount,
                PercentageChange = CalculatePercentageChange(todayCount, yesterdayCount)
            };
        }

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
            await _unitOfWork.VirtualInterviewRepository.CountAsync(i => i.Status == "in_progress");

        private double CalculatePercentageChange(int today, int yesterday)
        {
            if (yesterday == 0)
                return today > 0 ? 100 : 0;

            return ((double)(today - yesterday) / yesterday) * 100;
        }


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
        public async Task<List<UserListDto>> GetUserListAsync()
        {
            var users = await _unitOfWork.UserRepository
                .GetAllAsync(includeProperties: "UserRoleUsers");

            var result = new List<UserListDto>();

            foreach (var user in users)
            {
                var role = await _unitOfWork.UserRoleRepository.CheckRole(user);
                result.Add(new UserListDto
                {
                    Name = $"{user.LastName} {user.FirstName}",
                    Email = user.Email,
                    Role = role,
                    Status = user.AccountStatus,
                    DateCreated = user.CreatedAt
                });
            }

            return result;
        }
        public async Task<TimeSeriesAnalysisDto> GetInterviewSessionsAsync(DateTime from, DateTime to, string range)
        {
            var data = await _unitOfWork.VirtualInterviewRepository
                .GetAllAsync(i => i.CreatedAt >= from && i.CreatedAt <= to);

            return GroupTimeSeries(data.Select(i => i.CreatedAt!.Value), range);
        }

        public async Task<TimeSeriesAnalysisDto> GetCvAnalysesAsync(DateTime from, DateTime to, string range)
        {
            var data = await _unitOfWork.CvAnalysisResult
                .GetAllAsync(r => r.CreatedAt >= from && r.CreatedAt <= to);

            return GroupTimeSeries(data.Select(r => r.CreatedAt!), range);
        }
        public async Task<PremiumConversionRateDto> GetPremiumConversionRateAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllUsersWithSubscriptionsAsync();
            var totalUsers = users.Count;

            var groupStats = users.GroupBy(user =>
            {
                var activeSub = user.UserSubscriptions.FirstOrDefault();
                return activeSub?.Plan?.PlanName ?? "None";
            })
            .Select(g => new ConversionRateItemDto
            {
                PlanName = g.Key,
                UserCount = g.Count(),
                Percentage = Math.Round((double)g.Count() * 100 / totalUsers, 2)
            })
            .ToList();

            return new PremiumConversionRateDto
            {
                TotalUsers = totalUsers,
                Breakdown = groupStats
            };
        }
        public async Task<List<ScoreHistogramBinDto>> GetUserScoreHistogramAsync()
        {
            var interviews = await _unitOfWork.VirtualInterviewRepository.FindAsync(
                i => i.Status == "completed" && i.OverallScore != null);

            var bins = new List<ScoreHistogramBinDto>();
            for (decimal i = 0; i < 10; i += 0.5m)
            {
                decimal min = i;
                decimal max = i + 0.5m;

                var count = interviews.Count(iw =>
                    iw.OverallScore >= min && iw.OverallScore < max);

                bins.Add(new ScoreHistogramBinDto
                {
                    Range = $"{min:F1}-{max:F1}",
                    Count = count
                });
            }

            return bins;
        }
        private TimeSeriesAnalysisDto GroupTimeSeries(IEnumerable<DateTime> dates, string range)
        {
            IEnumerable<IGrouping<DateTime, DateTime>> grouped;

            switch (range.ToLower())
            {
                case "weekly":
                    grouped = dates
                        .GroupBy(d =>
                        {
                            var calendar = CultureInfo.InvariantCulture.Calendar;
                            int week = calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                            int year = d.Year;

                            // Get the first day of the year
                            DateTime jan1 = new DateTime(year, 1, 1);

                            // Calculate the first day of the target week
                            int daysOffset = (week - 1) * 7;
                            DateTime weekStart = jan1.AddDays(daysOffset);

                            // Adjust to Monday (start of the week)
                            while (weekStart.DayOfWeek != DayOfWeek.Monday)
                                weekStart = weekStart.AddDays(-1);

                            return weekStart.Date; // group key = Monday of that week
                        });
                    break;
                case "monthly":
                    grouped = dates.GroupBy(d => new DateTime(d.Year, d.Month, 1));
                    break;
                default:
                    grouped = dates.GroupBy(d => d.Date);
                    break;
            }

            var result = grouped
                .Select(g => new TimeSeriesPointDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new TimeSeriesAnalysisDto
            {
                Data = result,
                Peak = result.OrderByDescending(x => x.Count).FirstOrDefault(),
                Low = result.OrderBy(x => x.Count).FirstOrDefault()
            };
        }
    }
    public class WeekGroup
    {
        public int Week { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}
