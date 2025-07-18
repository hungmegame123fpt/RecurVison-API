﻿using AutoMapper;
using BusinessObject.DTO;
using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.InterviewQuestion;
using BusinessObject.DTO.VirtualInterview;
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
    public class VirtualInterviewService : IVirtualInterviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICVService _cVService;
        private readonly IAIClient _aiClient;
        private readonly IMapper _mapper;

        public VirtualInterviewService(IUnitOfWork unitOfWork, IMapper mapper, ICVService cVService, IAIClient aiClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cVService = cVService;
            _aiClient = aiClient;
        }
        public async Task<StartInterviewResponse> StartInterviewAsync(StartInterviewRequest request)
        {
            // 1. Tạo bản ghi Interview trước để có InterviewId (dùng làm session_id)
            var interview = new VirtualInterview
            {
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = "in_progress",
                JobDescription = request.JobDescription,
            };

            await _unitOfWork.VirtualInterviewRepository.CreateAsync(interview);
            await _unitOfWork.SaveChanges();

            var sessionId = interview.InterviewId.ToString();

            // 2. Gọi ParseCv API để lấy plain text
            var parsedCv = await _cVService.ParseCvAsync(request.CvId);
            if (parsedCv == null || string.IsNullOrWhiteSpace(parsedCv.PlainTextContent))
                throw new Exception("CV text not found or empty.");
            interview.CvContent = parsedCv.PlainTextContent;
            await _unitOfWork.VirtualInterviewRepository.UpdateAsync(interview);
            await _unitOfWork.SaveChanges();
            // 3. Gọi AI với cv_text + job_description + session_id (interviewId)
            var aiRequest = new AiSessionRequest
            {
                CleanedCvText = parsedCv.PlainTextContent,
                JobDescription = request.JobDescription,
                PreviousQuestions = new List<string>(), // bắt đầu mới
                SessionId = sessionId
            };

            var aiResponse = await _aiClient.StartSessionAsync(aiRequest);

            if (aiResponse?.Data?.Questions == null)
                throw new Exception("AI did not return any questions.");

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
                Questions = _mapper.Map<List<InterviewQuestionDto>>(aiResponse.Data.Questions),
                Analysis = aiResponse.Data.Analysis,
                NextFocusAreas = aiResponse.Data.NextFocusAreas,
                CleanCvText = parsedCv.PlainTextContent, 
                JobDescription = request.JobDescription,
            };
        }
        public async Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var question = await _unitOfWork.InterviewQuestionRepository.GetByIdAsync(request.QuestionId);
            if (question == null || question.InterviewId != request.InterviewId)
                throw new Exception("Invalid question or interview.");

            // Save user answer
            question.AnswerText = request.AnswerText;

            // Load data from interview
            var interview = await _unitOfWork.VirtualInterviewRepository.GetByIdAsync(request.InterviewId);
            if (interview == null)
                throw new Exception("Interview not found.");

           

            var aiRequest = new AiAnswerEvaluateRequest
            {
                SessionId = request.InterviewId.ToString(),
                AnswerText = request.AnswerText,
                CleanedCvText =interview.CvContent,
                JobDescription = interview.JobDescription,
                //PreviousQuestions = new List<string> { question.QuestionText ?? "" }
            };
            aiRequest.PreviousQuestions ??= new List<string>();
            var aiResponse = await _aiClient.EvaluateAnswerAsync(aiRequest);
            if (aiResponse?.Data?.Feedback == null)
                throw new Exception("AI feedback response invalid.");

            // Save AI feedback
            question.Feedback = aiResponse.Data.Feedback.Feedback;
            question.QuestionScore = (decimal?)aiResponse.Data.Feedback.Score;

            await _unitOfWork.InterviewQuestionRepository.UpdateAsync(question);
            await _unitOfWork.SaveChanges();

            return new SubmitAnswerResponse
            {
                QuestionId = question.QuestionId,
                InterviewId = question.InterviewId,
                Score = question.QuestionScore,
                Feedback = question.Feedback,
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
