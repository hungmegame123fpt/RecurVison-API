using BusinessObject.DTO;
using Microsoft.EntityFrameworkCore;
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
    public class CareerPlanService : ICareerPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CareerPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<NextCareerGoalDto?> GetNextCareerGoalAsync(int userId)
        {
            var plan = await _unitOfWork.CareerPlanRepository
                .GetAllQueryable()
                .Include(p => p.CareerMilestones)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
            var job = await _unitOfWork.JobPostingRepository.GetByUserIdAsync(userId);
            if (plan == null) return null;
            var baseDate = plan.LastUpdated ?? plan.CreatedAt;
            var milestone = plan.CareerMilestones
                .Where(m => m.AchievementStatus.Equals("pending"))
                .OrderBy(m => m.MilestoneId)
                .FirstOrDefault();
            string? targetDate = null;

            if (baseDate.HasValue && milestone?.TargetYear != null)
            {
                var estimatedDate = baseDate.Value.AddYears(milestone.TargetYear.Value);
                targetDate = estimatedDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            }
            return new NextCareerGoalDto
            {
                PlanId = plan.PlanId,
                MilestoneId = milestone?.MilestoneId,
                TargetTitle = milestone?.Title ?? "Land a new job with desired offer",
                TargetDate = targetDate,
                OverallPath = plan?.CareerGoal,
                CurrentPosition = job.JobPosition,
                Status = milestone.AchievementStatus
            };
        }
        public async Task<bool> UpdateCareerPlanAsync(int userId, UpdateCareerPlanRequest request)
        {
            var plan = await _unitOfWork.CareerPlanRepository
                 .GetAllQueryable()
                 .Include(p => p.CareerMilestones)
                 .Where(p => p.UserId == userId)
                 .OrderByDescending(p => p.CreatedAt)
                 .FirstOrDefaultAsync();
            var job = await _unitOfWork.JobPostingRepository.GetByUserIdAsync(userId);
            if (plan == null)
                throw new Exception("Career plan not found.");

            plan.CurrentPosition = job.JobPosition;
            plan.LastUpdated = DateTime.UtcNow;

            foreach (var mUpdate in request.Milestones)
            {
                var milestone = plan.CareerMilestones.FirstOrDefault(m => m.MilestoneId == mUpdate.MilestoneId);
                if (milestone != null)
                {
                    milestone.AchievementStatus = mUpdate.AchievementStatus;
                }
            }

            await _unitOfWork.CareerPlanRepository.UpdateAsync(plan);
            await _unitOfWork.SaveChanges();
            return true;
        }
        public async Task<List<CareerMilestoneResponse>> GetByPlanIdAsync(int planId)
        {
            var milestones = await _unitOfWork.CareerPlanRepository.GetByPlanIdAsync(planId);

            return milestones.Select(m => new CareerMilestoneResponse
            {
                Title = m.Title,
                YearToComplete = m.TargetYear
            }).ToList();
        }
        public async Task<bool> DeleteCareerPlanAsync(int planId)
        {
            var plan = await _unitOfWork.CareerPlanRepository.GetByIdAsync(planId);
            if (plan != null)
            {
               await _unitOfWork.CareerPlanRepository.DeleteAsync(plan);
               await _unitOfWork.SaveChanges();
               return true;
            }
            return false;
        }
    }
}
