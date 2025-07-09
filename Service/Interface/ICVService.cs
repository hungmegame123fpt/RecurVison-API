using BusinessObject.DTO;
using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICVService
    {
        Task<CVImportResponse> ImportCvAsync(CVImportRequest request);
        Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisResultRequest cvAnalysis);

		Task<CVExportResponse> ExportCvAsync(CVExportRequest request);
        Task<List<string>> GetSupportedImportFormats();
        Task<List<string>> GetSupportedExportFormats();
        Task<ParsedDocumentResult> ParseCvAsync(int userId, int cvId);
        Task<ParseCvResponse> ParseCvFromUrlAsync(string fileUrl, bool includeMetadata = true);
        Task<List<CVDto>> GetAllCvAsync();
        Task<CvListResponse> GetUserCvsAsync(int userId);
        Task<CvDetailResponse> GetCvByIdAsync(int userId, int cvId);
        Task<DeleteResponse> DeleteCvAsync(int userId, int cvId);
        Task<byte[]> EditPdfAsync(byte[] originalPdf, string newText);
        Task<string?> CategorizeCvByFieldAsync(int cvId, string plainTextContent);
        Task<CvAnalysisResponse> ImportCvAnalysisJsonAsync(CvAnalysisRequest request);
    }
}
