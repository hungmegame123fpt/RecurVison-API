using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IFeedbackService
    {
        Task AddFeedbackAsync(CreateFeedbackRequest request, int userId);
        Task<List<FeedbackResponse>> GetFeedbacksByUserAsync(int userId);
        Task<IEnumerable<FeedbackResponse>> GetAllAsync();
        Task UpdateAsync(int userId, UpdateFeedbackRequest request);
        Task<FeedbackStatsResponse> GetRatingStatisticsAsync();
    }
}
