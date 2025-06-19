using Microsoft.AspNetCore.Http;
using Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class LocalFIleService : IFileStorageService
    {
        private readonly string _storagePath;
        private readonly ILogger<FileStorageService> _logger;

        public LocalFIleService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            _storagePath = configuration["FileStorage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            _logger = logger;

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string fileName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty or null");

                // Your CVService already generates unique filenames, so use it as-is
                var fullPath = Path.Combine(_storagePath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                _logger.LogInformation("File saved successfully: {FilePath}", fullPath);
                return fullPath; // Return full path as your CVService expects
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<byte[]?> GetFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found: {FilePath}", filePath);
                    return null;
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                    return false;
                }

                await Task.Run(() => File.Delete(filePath));
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        //public async Task<bool> FileExistsAsync(string filePath)
        //{
        //    return await Task.FromResult(File.Exists(filePath));
        //}

        //public async Task<FileInfo?> GetFileInfoAsync(string filePath)
        //{
        //    try
        //    {
        //        if (!File.Exists(filePath))
        //            return null;

        //        return await Task.FromResult(new FileInfo(filePath));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting file info: {FilePath}", filePath);
        //        return null;
        //    }
        //}

    }
}
