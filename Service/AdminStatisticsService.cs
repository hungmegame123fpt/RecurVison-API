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
    }
}
