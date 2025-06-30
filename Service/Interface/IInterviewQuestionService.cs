using BusinessObject.DTO.InterviewQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IInterviewQuestionService
    {
        Task<InterviewQuestionDto> CreateQuestionAsync(CreateInterviewQuestionDto createDto);
        Task<List<InterviewQuestionDto>> GetQuestionsByInterviewIdAsync(int interviewId);
        Task<InterviewQuestionDto?> GetQuestionByIdAsync(int questionId);
        Task<InterviewQuestionDto> UpdateAnswerAsync(int questionId, UpdateAnswerDto updateDto);
        Task<InterviewQuestionDto> UpdateQuestionScoreAsync(int questionId, UpdateQuestionScoreDto updateDto);
        Task<bool> DeleteQuestionAsync(int questionId);
        Task<List<InterviewQuestionDto>> GetUnansweredQuestionsAsync(int interviewId);
    }
}
