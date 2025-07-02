using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.VirtualInterview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IVirtualInterviewService
    {
        Task<StartInterviewResponse> StartInterviewAsync(StartInterviewRequest request);
        Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request);
        Task<VirtualInterviewDto> CreateInterviewAsync(CreateVirtualInterviewDto createDto);
        Task<VirtualInterviewDto?> GetInterviewByIdAsync(int interviewId);
        Task<List<VirtualInterviewDto>> GetInterviewsByUserIdAsync(int userId);
        Task<List<VirtualInterviewDto>> GetInterviewsByJobIdAsync(int jobId);
        Task<VirtualInterviewDto> UpdateInterviewStatusAsync(int interviewId, UpdateInterviewStatusDto updateDto);
        Task<bool> DeleteInterviewAsync(int interviewId);
        Task<List<VirtualInterviewDto>> GetInterviewsByStatusAsync(string status);
        Task<VirtualInterviewDto?> GetInterviewWithQuestionsAsync(int interviewId);
    }
}
