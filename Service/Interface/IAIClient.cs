using BusinessObject.DTO.AiClient;
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

    }
}
