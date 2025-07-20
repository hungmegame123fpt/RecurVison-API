using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IInterviewQuestionRepository : IBaseRepository<InterviewQuestion>
    {
        Task<List<InterviewQuestion>> GetQuestionsByInterviewIdAsync(int interviewId);
        Task<List<InterviewQuestion>> GetQuestionsBySessionIdAsync(string sessionId);
        Task<InterviewQuestion?> GetQuestionWithAnswerAsync(int questionId);
        Task<List<InterviewQuestion>> GetUnansweredQuestionsAsync(int interviewId);
    }
}
