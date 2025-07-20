using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IVirtualInterviewRepository : IBaseRepository<VirtualInterview>
    {
        Task<List<VirtualInterview>> GetInterviewsByUserIdAsync(int userId);
        Task<List<VirtualInterview>> GetInterviewsByJobIdAsync(int jobId);
        Task<VirtualInterview?> GetInterviewWithQuestionsAsync(int interviewId);
        Task<List<VirtualInterview>> GetCompletedInterviewsAsync();
        Task<List<VirtualInterview>> GetInterviewsByStatusAsync(string status);
        Task<int> CountCompletedInterviewsAsync(DateTime date);
    }
}
