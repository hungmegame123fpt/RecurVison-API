using AutoMapper;
using BusinessObject.DTO;
using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.InterviewQuestion;
using BusinessObject.DTO.VirtualInterview;
using BusinessObject.Entities;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class VirtualInterviewService : IVirtualInterviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICVService _cVService;
        private readonly IUserSubscriptionService _subscriptionService;
        private readonly IAIClient _aiClient;
        private readonly IMapper _mapper;

        public VirtualInterviewService(IUnitOfWork unitOfWork, IMapper mapper, ICVService cVService, IAIClient aiClient, IUserSubscriptionService subscriptionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cVService = cVService;
            _aiClient = aiClient;
            _subscriptionService = subscriptionService;
        }
        public async Task<StartInterviewResponse> StartInterviewAsync(StartInterviewRequest request)
        {
            // 2. Gọi ParseCv API để lấy plain text
            var parsedCv = await _cVService.ParseCvAsync(request.CvId);
            if (parsedCv == null || string.IsNullOrWhiteSpace(parsedCv.PlainTextContent))
                throw new Exception("CV text not found or empty.");
            var cv = await _cVService.GetCvByIdAsync(request.CvId);
            // 3. Gọi AI với cv_text + job_description + session_id (interviewId)
            var aiRequest = new AiSessionRequest
            {
                CvFileUrl = cv.Cv.FilePath,
                JobDescription = request.JobDescription,
            };

            var aiResponse = await _aiClient.StartSessionAsync(aiRequest);
            if (aiResponse?.Data?.Questions == null)
                throw new Exception("AI did not return any questions.");
            var interview = new VirtualInterview
            {
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = "in_progress",
                JobDescription = request.JobDescription,
                SessionId = aiResponse.Data.SessionId,
                CvContent = parsedCv.PlainTextContent
            };

            await _unitOfWork.VirtualInterviewRepository.CreateAsync(interview);
            await _unitOfWork.SaveChanges();
            // 4. Lưu các câu hỏi AI trả về
            foreach (var q in aiResponse.Data.Questions)
            {
                var question = new InterviewQuestion
                {
                    InterviewId = interview.InterviewId,
                    QuestionText = q.Question,
                    Feedback = null,
                    QuestionScore = null
                };
                await _unitOfWork.InterviewQuestionRepository.CreateAsync(question);
            }

            await _unitOfWork.SaveChanges();

            return new StartInterviewResponse
            {
                InterviewId = interview.InterviewId,
                SessionId = interview.SessionId,
                Questions = _mapper.Map<List<InterviewQuestionDto>>(aiResponse.Data.Questions)
            };
        }
        public async Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var questions = await _unitOfWork.InterviewQuestionRepository.GetQuestionsBySessionIdAsync(request.SessionId);
            var currentQuestion = questions.OrderByDescending(q => q.QuestionId).FirstOrDefault();
            if (currentQuestion == null)
                throw new Exception("Invalid question or interview.");

            // Save user answer
            currentQuestion.AnswerText = request.AnswerText;

            // Load data from interview
            var aiResponse = await _aiClient.EvaluateAnswerAsync(request);
            if (aiResponse?.Data?.Feedback == null)
                throw new Exception("AI feedback response invalid.");

            // Save AI feedback
            currentQuestion.Feedback = aiResponse.Data.Feedback.Feedback;
            currentQuestion.QuestionScore = (decimal?)aiResponse.Data.Feedback.Score;
            await _unitOfWork.InterviewQuestionRepository.UpdateAsync(currentQuestion);
            await _unitOfWork.SaveChanges();
            var interview = await _unitOfWork.VirtualInterviewRepository.GetByIdAsync(currentQuestion.InterviewId);
            if (interview == null)
                throw new Exception("Interview not found.");           
            // Count number of answered questions (i.e., iterations)
            var answeredCount = questions.Count(q => !string.IsNullOrWhiteSpace(q.AnswerText));
            // When iteration hits 5, finalize interview
            if (answeredCount >= 4)
            {
                interview.Status = "completed";
                interview.OverallScore = (decimal?)aiResponse.Data.Feedback.Score; // Or average score of all answers
                await _unitOfWork.VirtualInterviewRepository.UpdateAsync(interview);
                await _unitOfWork.SaveChanges();
                var subscription = await _subscriptionService.GetUserActiveSubscriptionAsync(interview.UserId);
                if (subscription == null)
                    throw new Exception("subscription not found");
                if (subscription.InterviewPerDayRemaining <= 0)
                    throw new Exception("Your daily Cv analysis has depleted");
                subscription.InterviewPerDayRemaining -= 1;
                await _subscriptionService.UpdateSubscriptionAsync(subscription.SubscriptionId, subscription);
            }
            else
            {
                var question = new InterviewQuestion
                {
                    InterviewId = interview.InterviewId,
                    QuestionText = aiResponse.Data.NextQuestion.Question,
                    Feedback = null,
                    QuestionScore = null
                };
                await _unitOfWork.InterviewQuestionRepository.CreateAsync(question);
                await _unitOfWork.SaveChanges();
            }
            return new SubmitAnswerResponse
            {
                QuestionId = currentQuestion.QuestionId,
                InterviewId = currentQuestion.InterviewId,
                Score = currentQuestion.QuestionScore,
                Feedback = currentQuestion.Feedback,
                Summary = aiResponse.Data.Feedback.Summary,
                NextQuestion = aiResponse.Data.NextQuestion
            };
        }
        public async Task<VirtualInterviewDto> CreateInterviewAsync(CreateVirtualInterviewDto createDto)
        {
            var interview = new VirtualInterview
            {
                UserId = createDto.UserId,
                JobId = createDto.JobId,
                CreatedAt = DateTime.UtcNow,
                Status = "Created"
            };

            var createdInterview = await _unitOfWork.VirtualInterviewRepository.CreateAsync(interview);
            await _unitOfWork.VirtualInterviewRepository.SaveChangesAsync();

            return _mapper.Map<VirtualInterviewDto>(createdInterview);
        }

        public async Task<VirtualInterviewDto?> GetInterviewByIdAsync(int interviewId)
        {
            var interview = await _unitOfWork.VirtualInterviewRepository.GetByIdAsync(interviewId);
            return interview != null ? _mapper.Map<VirtualInterviewDto>(interview) : null;
        }

        public async Task<List<VirtualInterviewDto>> GetInterviewsByUserIdAsync(int userId)
        {
            var interviews = await _unitOfWork.VirtualInterviewRepository.GetInterviewsByUserIdAsync(userId);
            return _mapper.Map<List<VirtualInterviewDto>>(interviews);
        }

        public async Task<List<VirtualInterviewDto>> GetInterviewsByJobIdAsync(int jobId)
        {
            var interviews = await _unitOfWork.VirtualInterviewRepository.GetInterviewsByJobIdAsync(jobId);
            return _mapper.Map<List<VirtualInterviewDto>>(interviews);
        }

        public async Task<VirtualInterviewDto> UpdateInterviewStatusAsync(int interviewId, UpdateInterviewStatusDto updateDto)
        {
            var interview = await _unitOfWork.VirtualInterviewRepository.GetByIdAsync(interviewId);
            if (interview == null)
                throw new KeyNotFoundException($"Interview with ID {interviewId} not found");

            interview.Status = updateDto.Status;
            interview.OverallScore = updateDto.OverallScore;
            interview.RecordingPath = updateDto.RecordingPath ?? interview.RecordingPath;

            await _unitOfWork.VirtualInterviewRepository.UpdateAsync(interview);
            await _unitOfWork.VirtualInterviewRepository.SaveChangesAsync();

            return _mapper.Map<VirtualInterviewDto>(interview);
        }

        public async Task<bool> DeleteInterviewAsync(int interviewId)
        {
            var interview = await _unitOfWork.VirtualInterviewRepository.GetByIdAsync(interviewId);
            if (interview == null)
                return false;

            await _unitOfWork.VirtualInterviewRepository.DeleteAsync(interview);
            await _unitOfWork.VirtualInterviewRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<VirtualInterviewDto>> GetInterviewsByStatusAsync(string status)
        {
            var interviews = await _unitOfWork.VirtualInterviewRepository.GetInterviewsByStatusAsync(status);
            return _mapper.Map<List<VirtualInterviewDto>>(interviews);
        }

        public async Task<VirtualInterviewDto?> GetInterviewWithQuestionsAsync(int interviewId)
        {
            var interview = await _unitOfWork.VirtualInterviewRepository.GetInterviewWithQuestionsAsync(interviewId);
            return interview != null ? _mapper.Map<VirtualInterviewDto>(interview) : null;
        }
    }
}
