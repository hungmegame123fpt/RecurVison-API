using BusinessObject.DTO;
using BusinessObject.DTO.AiClient;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAIClient
    {
        Task<AiSessionResponse?> StartSessionAsync(AiSessionRequest request);
        Task<AiAnswerEvaluateResponse> EvaluateAnswerAsync(AiAnswerEvaluateRequest request);
        Task<AiCvAnalysisResponse> AnalyzeCvAsync(int Cvid, IFormFile jdFile);
        Task<List<SuggestedJob>> GetSuggestedJobsForCvAsync(int cvId);



    }
}
