using BusinessObject.DTO;
using BusinessObject.DTO.CV;
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
        Task<CVExportResponse> ExportCvAsync(CVExportRequest request);
        Task<List<string>> GetSupportedImportFormats();
        Task<List<string>> GetSupportedExportFormats();
        Task<ParseCvResponse> ParseCvAsync(ParseCvRequest request);
        Task<ParseCvResponse> ParseCvFromUrlAsync(string fileUrl, bool includeMetadata = true);
        Task<CvListResponse> GetUserCvsAsync(int userId);
        Task<CvDetailResponse> GetCvByIdAsync(int userId, int cvId);
        Task<DeleteResponse> DeleteCvAsync(int userId, int cvId);
    }
}
