using BusinessObject.DTO;
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
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddFeedbackAsync(CreateFeedbackRequest request, int userId)
        {
            var feedback = new Feedback
            {
                UserId = userId,
                Content = request.Content,
                Rating = request.Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FeedbackRepository.CreateAsync(feedback);
            await _unitOfWork.SaveChanges();
        }

        public async Task<List<FeedbackResponse>> GetFeedbacksByUserAsync(int userId)
        {
            var list = await _unitOfWork.FeedbackRepository.GetAllAsync(
            filter: f => f.UserId == userId );
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            return list.Select(f => new FeedbackResponse
            {
                FeedbackId = f.FeedbackId,
                UserEmail = user.Email,
                Content = f.Content,
                Rating = f.Rating,
                CreatedAt = f.CreatedAt
            }).ToList();
        }
        public async Task<IEnumerable<FeedbackResponse>> GetAllAsync()
        {
            var items = await _unitOfWork.FeedbackRepository.GetAllAsync(
         includeProperties: "User"
     );
            return items.Select(f => new FeedbackResponse
            {
                FeedbackId = f.FeedbackId,
                UserEmail = f.User.Email,
                Content = f.Content,
                Rating = f.Rating,
                CreatedAt = f.CreatedAt
            });
        }

        public async Task UpdateAsync(int userId, UpdateFeedbackRequest request)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(request.FeedbackId);
            if (feedback == null || feedback.UserId != userId)
                throw new UnauthorizedAccessException("Cannot update this feedback.");

            feedback.Content = request.Content;
            feedback.Rating = request.Rating;
            await _unitOfWork.SaveChanges();
        }
        public async Task<FeedbackStatsResponse> GetRatingStatisticsAsync()
        {
            var allFeedback = await _unitOfWork.FeedbackRepository.GetAllAsync(); 
            var ratings = allFeedback.Where(f => f.Rating.HasValue).Select(f => f.Rating!.Value).ToList();

            var breakdown = ratings
                .GroupBy(r => r)
                .ToDictionary(g => g.Key, g => g.Count());

            var average = ratings.Count > 0 ? Math.Round(ratings.Average(), 2) : 0;

            return new FeedbackStatsResponse
            {
                AverageRating = average,
                TotalRatings = ratings.Count,
                RatingBreakdown = breakdown
            };
        }
    }
}
