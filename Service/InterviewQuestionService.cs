using AutoMapper;
using BusinessObject.DTO.InterviewQuestion;
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
    public class InterviewQuestionService : IInterviewQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InterviewQuestionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<InterviewQuestionDto> CreateQuestionAsync(CreateInterviewQuestionDto createDto)
        {
            var question = new InterviewQuestion
            {
                InterviewId = createDto.InterviewId,
                QuestionText = createDto.QuestionText
            };

            var createdQuestion = await _unitOfWork.InterviewQuestionRepository.CreateAsync(question);
            await _unitOfWork.InterviewQuestionRepository.SaveChangesAsync();

            return _mapper.Map<InterviewQuestionDto>(createdQuestion);
        }

        public async Task<List<InterviewQuestionDto>> GetQuestionsByInterviewIdAsync(int interviewId)
        {
            var questions = await _unitOfWork.InterviewQuestionRepository.GetQuestionsByInterviewIdAsync(interviewId);
            return _mapper.Map<List<InterviewQuestionDto>>(questions);
        }

        public async Task<InterviewQuestionDto?> GetQuestionByIdAsync(int questionId)
        {
            var question = await _unitOfWork.InterviewQuestionRepository.GetByIdAsync(questionId);
            return question != null ? _mapper.Map<InterviewQuestionDto>(question) : null;
        }

        public async Task<InterviewQuestionDto> UpdateAnswerAsync(int questionId, UpdateAnswerDto updateDto)
        {
            var question = await _unitOfWork.InterviewQuestionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException($"Question with ID {questionId} not found");

            question.AnswerText = updateDto.AnswerText;

            await _unitOfWork.InterviewQuestionRepository.UpdateAsync(question);
            await _unitOfWork.InterviewQuestionRepository.SaveChangesAsync();

            return _mapper.Map<InterviewQuestionDto>(question);
        }

        public async Task<InterviewQuestionDto> UpdateQuestionScoreAsync(int questionId, UpdateQuestionScoreDto updateDto)
        {
            var question = await _unitOfWork.InterviewQuestionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException($"Question with ID {questionId} not found");

            question.QuestionScore = updateDto.QuestionScore;
            question.Feedback = updateDto.Feedback;

            await _unitOfWork.InterviewQuestionRepository.UpdateAsync(question);
            await _unitOfWork.InterviewQuestionRepository.SaveChangesAsync();

            return _mapper.Map<InterviewQuestionDto>(question);
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _unitOfWork.InterviewQuestionRepository.GetByIdAsync(questionId);
            if (question == null)
                return false;

            await _unitOfWork.InterviewQuestionRepository.DeleteAsync(question);
            await _unitOfWork.InterviewQuestionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<InterviewQuestionDto>> GetUnansweredQuestionsAsync(int interviewId)
        {
            var questions = await _unitOfWork.InterviewQuestionRepository.GetUnansweredQuestionsAsync(interviewId);
            return _mapper.Map<List<InterviewQuestionDto>>(questions);
        }
    }

}
