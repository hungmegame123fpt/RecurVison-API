using Microsoft.AspNetCore.Http;
using Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using HttpMethod = System.Net.Http.HttpMethod;
using BusinessObject.DTO.CV;
namespace Service
{
    public class FileStorageService : IFileStorageService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinaryService;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _folder;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            _logger = logger;
            _folder = configuration["Cloudinary:Folder"] ?? "cv-uploads";

            // Initialize Cloudinary
            var cloudinaryUrl = configuration["Cloudinary:CloudinaryUrl"];
            if (!string.IsNullOrEmpty(cloudinaryUrl))
            {
                _cloudinaryService = new CloudinaryDotNet.Cloudinary(cloudinaryUrl);
            }
            else
            {
                var account = new Account(
                    configuration["Cloudinary:CloudName"],
                    configuration["Cloudinary:ApiKey"],
                    configuration["Cloudinary:ApiSecret"]
                );
                _cloudinaryService = new CloudinaryDotNet.Cloudinary(account);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string fileName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty or null");
                // Generate unique public ID for Cloudinary
                var publicId = GenerateCloudinaryPublicId(fileName);
                using var stream = file.OpenReadStream();
                UploadResult uploadResult;
                var fileExtension = Path.GetExtension(fileName).ToLower();
                if (fileExtension == ".mp4" || fileExtension == ".mov" || file.ContentType.StartsWith("video"))
                {
                    var uploadParams = new VideoUploadParams()
                    {
                        File = new FileDescription(fileName, stream),
                        PublicId = publicId,
                        Folder = _folder,
                        UseFilename = false,
                        UniqueFilename = true,
                        AccessMode = "public"
                    };
                    uploadResult = await _cloudinaryService.UploadAsync(uploadParams);
                }
                else
                {
                    var uploadParams = new RawUploadParams()
                    {
                        File = new FileDescription(fileName, stream),
                        PublicId = publicId,
                        Folder = _folder,
                        UseFilename = false,
                        UniqueFilename = true,
                        AccessMode = "public",
                        Type = "upload"
                    };
                    uploadResult = await _cloudinaryService.UploadAsync(uploadParams);
                }
                if (uploadResult.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", uploadResult.Error.Message);
                    throw new Exception($"Failed to upload file to Cloudinary: {uploadResult.Error.Message}");
                }
                _logger.LogInformation("File uploaded successfully to Cloudinary: {PublicId}", uploadResult.PublicId);
                // Return the secure URL which can be used to access the file
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Cloudinary: {FileName}", fileName);
                throw;
            }
        }
        public async Task<string> SaveJsonFileWithOriginalNameAsync(IFormFile file, string originalFileName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty or null");

                var publicId = Path.GetFileNameWithoutExtension(originalFileName); 
                using var stream = file.OpenReadStream();

                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(originalFileName, stream),
                    PublicId = publicId,
                    Folder = "cv-analysis/json", // có thể cấu hình
                    UseFilename = true,
                    UniqueFilename = false,
                    Type = "upload",
                    AccessMode = "public"
                };

                var uploadResult = await _cloudinaryService.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", uploadResult.Error.Message);
                    throw new Exception($"Upload to Cloudinary failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading JSON file: {FileName}", originalFileName);
                throw;
            }
        }

        public async Task<byte[]?> GetFileAsync(string filePath)
            {
                try
                {
                    // filePath is actually the Cloudinary URL
                    if (string.IsNullOrEmpty(filePath))
                    {
                        _logger.LogWarning("File path is null or empty");
                        return null;
                    }

                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "CVExporter/1.0");
                    httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                    var response = await httpClient.GetAsync(filePath);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("File not found at URL: {FilePath}", filePath);
                        return null;
                    }

                    var fileContent = await response.Content.ReadAsByteArrayAsync();
                    _logger.LogInformation("File downloaded successfully from Cloudinary: {FilePath}", filePath);

                    return fileContent;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error downloading file from Cloudinary: {FilePath}", filePath);
                    throw;
                }
            }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("File path is null or empty for deletion");
                    return false;
                }

                // Extract public ID from the Cloudinary URL
                var publicId = ExtractPublicIdFromUrl(filePath);
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("Could not extract public ID from URL: {FilePath}", filePath);
                    return false;
                }

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Raw
                };

                var deleteResult = await _cloudinaryService.DestroyAsync(deleteParams);

                if (deleteResult.Error != null)
                {
                    _logger.LogError("Cloudinary delete error: {Error}", deleteResult.Error.Message);
                    return false;
                }

                var success = deleteResult.Result == "ok";
                if (success)
                {
                    _logger.LogInformation("File deleted successfully from Cloudinary: {PublicId}", publicId);
                }
                else
                {
                    _logger.LogWarning("File deletion failed or file not found: {PublicId}", publicId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Cloudinary: {FilePath}", filePath);
                return false;
            }
        }

        // Additional helper methods
        public async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                using var httpClient = new HttpClient();
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, filePath));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
                return false;
            }
        }

        private string GenerateCloudinaryPublicId(string fileName)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];

            return $"{_folder}/{nameWithoutExtension}_{timestamp}_{guid}{extension}";
        }

        private string ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            try
            {
                // Cloudinary URL format: https://res.cloudinary.com/{cloud_name}/{resource_type}/upload/v{version}/{public_id}.{format}
                var uri = new Uri(cloudinaryUrl);
                var pathSegments = uri.AbsolutePath.Split('/');

                // Find the upload segment and get everything after it
                var uploadIndex = Array.IndexOf(pathSegments, "upload");
                if (uploadIndex == -1 || uploadIndex >= pathSegments.Length - 1)
                    return string.Empty;

                // Skip version if present (starts with 'v' followed by numbers)
                var startIndex = uploadIndex + 1;
                if (startIndex < pathSegments.Length && pathSegments[startIndex].StartsWith("v") &&
                    pathSegments[startIndex].Length > 1 && char.IsDigit(pathSegments[startIndex][1]))
                {
                    startIndex++;
                }

                if (startIndex >= pathSegments.Length)
                    return string.Empty;

                // Join remaining segments to form public ID
                var publicIdParts = pathSegments.Skip(startIndex);
                var publicId = string.Join("/", publicIdParts);
                return publicId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting public ID from URL: {Url}", cloudinaryUrl);
                return string.Empty;
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
