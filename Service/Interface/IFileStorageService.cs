using BusinessObject.DTO.CV;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string fileName);
        Task<byte[]?> GetFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> SaveJsonFileWithOriginalNameAsync(IFormFile file, string originalFileName);
    }
}
