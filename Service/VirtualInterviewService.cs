using AutoMapper;
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
        private readonly IMapper _mapper;

        public VirtualInterviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
