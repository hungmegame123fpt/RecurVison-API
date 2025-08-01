﻿using BusinessObject.DTO;
using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Service
{
    public class CVService : ICVService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDocumentParserService _documentParserService;
        private readonly IUserSubscriptionService _subscriptionService;
        private readonly IAIClient _aiClient;
        private readonly ILogger<CVService> _logger;

        private readonly string[] _supportedImportFormats = { ".pdf", ".docx", ".doc", ".txt" };
        private readonly string[] _supportedExportFormats = { "pdf", "docx", "txt" };

        public CVService(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            IDocumentParserService documentParserService,
            ILogger<CVService> logger,
            IAIClient aiClient,
            IUserSubscriptionService subscriptionService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _documentParserService = documentParserService;
            _logger = logger;
            _aiClient = aiClient;
            _subscriptionService = subscriptionService;
        }
        public async Task<List<CVDto>> GetAllCvAsync()
        {
            return await _unitOfWork.CVRepository.GetAllAsync();
        }
        public async Task<CVImportResponse> ImportCvAsync(CVImportRequest request)
        {
            string tempFilePath = null;
            try
            {
                // Validate file
                var validationResult = ValidateImportFile(request.File);
                if (!validationResult.IsValid)
                {
                    return new CVImportResponse
                    {
                        Success = false,
                        Message = validationResult.ErrorMessage
                    };
                }

                // Create temporary file for parsing
                tempFilePath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }
                var fileExtension = Path.GetExtension(request.File.FileName);

                // Parse document content from local temp file with explicit file type
                var parsedContent = await _documentParserService.ParseDocumentAsync(tempFilePath, fileExtension);
                // Now upload to Cloudinary for storage
                var fileName = GenerateUniqueFileName(request.File.FileName);
                var filePath = await _fileStorageService.SaveFileAsync(request.File, fileName);
                Cv cv;
                CvVersion? version = null;

                if (request.CreateNewVersion && request.ExistingCvId.HasValue)
                {
                    // Create new version of existing CV
                    cv = await _unitOfWork.CVRepository.GetByUserIdAsync(request.UserId, request.ExistingCvId.Value);
                    if (cv == null)
                    {
                        return new CVImportResponse
                        {
                            Success = false,
                            Message = "Existing CV not found or access denied"
                        };
                    }

                    var latestVersion = await _unitOfWork.CVRepository.GetLatestVersionAsync(cv.CvId);
                    var newVersionNumber = (latestVersion?.VersionNumber ?? 0) + 1;

                    version = new CvVersion
                    {
                        CvId = cv.CvId,
                        VersionNumber = newVersionNumber,
                        FilePath = filePath,
                        ChangeSummary = "Imported new version"
                    };

                    version = await _unitOfWork.CVRepository.CreateVersionAsync(version);
                    cv.Title = request.Title;
                    cv.CurrentVersion = newVersionNumber;
                    cv.FilePath = filePath;
                    cv = await _unitOfWork.CVRepository.UpdateAsync(cv);
                }
                else
                {
                    // Create new CV
                    cv = new Cv
                    {
                        UserId = request.UserId,
                        Title = request.Title ?? Path.GetFileNameWithoutExtension(request.File.FileName),
                        FilePath = filePath
                    };

                    cv = await _unitOfWork.CVRepository.CreateAsync(cv);

                    // Create initial version
                    version = new CvVersion
                    {
                        CvId = cv.CvId,
                        VersionNumber = 1,
                        FilePath = filePath,
                        ChangeSummary = "Initial import"
                    };

                    version = await _unitOfWork.CVRepository.CreateVersionAsync(version);
                }

                var metadata = new CVMetadata
                {
                    OriginalFileName = request.File.FileName,
                    FileSize = request.File.Length,
                    FileType = Path.GetExtension(request.File.FileName),
                    ProcessedAt = DateTime.UtcNow,
                    ExtractedSections = parsedContent.ExtractedSections
                };

                _logger.LogInformation($"Successfully imported CV {cv.CvId} for user {request.UserId}");

                return new CVImportResponse
                {
                    Success = true,
                    Message = "CV imported successfully",
                    CvId = cv.CvId,
                    VersionId = version?.VersionId,
                    Metadata = metadata,
                    Warnings = parsedContent.Warnings
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing CV for user {request.UserId}");
                return new CVImportResponse
                {
                    Success = false,
                    Message = "An error occurred while importing the CV"
                };
            }
        }
		public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisResultRequest cvAnalysis)
		{
			// 1. Check CV exists
			var cv = await _unitOfWork.CVRepository.GetByIdAsync(cvAnalysis.CvId);
			if (cv == null)
				throw new Exception("CV not found");
            var originalFileName = cvAnalysis.jdFile.FileName; // This is the original name from the user
            var cloudinaryFileName = GenerateUniqueFileName(originalFileName); // Unique name for Cloudinary
            var filePath = await _fileStorageService.SaveFileAsync(cvAnalysis.jdFile, cloudinaryFileName);
            var jobDescription = new JobDescription
            {
                FileName = originalFileName,           
                FileUrl = filePath,                   
                UploadedAt = DateTime.UtcNow
            };
            await _unitOfWork.JobDescriptionRepository.CreateAsync(jobDescription);
            await _unitOfWork.SaveChanges();
            // 2. Check user subscription (optional: quota check if needed)
            var subscription = await _subscriptionService.GetUserActiveSubscriptionAsync(cv.UserId);
            if (subscription == null)
                throw new Exception("subscription not found");
            if (subscription.CvRemaining <= 0)
                throw new Exception("Your daily Cv analysis has depleted");
            subscription.CvRemaining -= 1;
            await _subscriptionService.UpdateSubscriptionAsync(subscription.SubscriptionId, subscription);
            // 3. Call AI Client
            var aiResponse = await _aiClient.AnalyzeCvAsync(cvAnalysis.CvId, cvAnalysis.jdFile);
			if (aiResponse?.Data == null)
				throw new Exception("Failed to analyze CV via AI");

			var analysisData = aiResponse.Data.CvAnalysisResult;

            // 4. Map AI response to your CvAnalysisResult entity
            var result = new CvAnalysisResult
            {
                Name = analysisData.Name,
                Email = analysisData.Email,
                Phone = analysisData.Phone,
                Summary = analysisData.Summary,
                JdAlignment = aiResponse.Data.JdAlignment,
                MatchScore = ExtractMatchScore(aiResponse.Data.JdAlignment),
                JobDescriptionId = jobDescription.Id,
                CreatedAt = DateTime.UtcNow,
                CvId = cv.CvId,
                Cv = cv,
                JobDescription = jobDescription
            };

            // Now assign navigation-based collections
            result.Skills = analysisData.Skills?.Select(skill => new CvSkill
            {
                SkillName = skill,
                CvAnalysisResult = result
            }).ToList() ?? new List<CvSkill>();

            result.Education = analysisData.Education?.Select(e => new CvEducation
            {
                Degree = e.Degree,
                Institution = e.Institution,
                StartYear = e.StartYear,
                EndYear = e.EndYear,
                Description = e.Description,
                CvAnalysisResult = result
            }).ToList() ?? new List<CvEducation>();

            result.Projects = analysisData.Projects?.Select(p => new CvProject
            {
                Title = p.Title,
                Description = p.Description,
                TechStacks = p.TechStacks?.Select(t => new CvProjectTechStack
                {
                    TechName = t.TechName
                }).ToList(),
                CvAnalysisResult = result
            }).ToList() ?? new List<CvProject>();

            result.Certifications = analysisData.Certifications?.Select(c => new CvCertification
            {
                Name = c.Name,
                Issuer = c.Issuer,
                TimePeriod = c.TimePeriod,
                Description = c.Description,
                CvAnalysisResult = result
            }).ToList() ?? new List<CvCertification>();

            // 5. Save to DB
            await _unitOfWork.CvAnalysisResult.CreateAsync(result);
			await _unitOfWork.CvAnalysisResult.SaveChangesAsync();

			return result;
		}
		public async Task<CVExportResponse> ExportCvAsync(CVExportRequest request)
        {
            try
            {
                // Get CV
                var cv = await _unitOfWork.CVRepository.GetByUserIdAsync(request.UserId, request.CvId);
                if (cv == null)
                {
                    return new CVExportResponse
                    {
                        Success = false,
                        Message = "CV not found or access denied"
                    };
                }

                // Get file content
                var fileContent = await _fileStorageService.GetFileAsync(cv.FilePath);
                if (fileContent == null)
                {
                    return new CVExportResponse
                    {
                        Success = false,
                        Message = "CV file not found in storage"
                    };
                }

                // Convert to requested format if needed
                var exportContent = await ConvertToFormat(fileContent, cv.FilePath, request.ExportFormat);

                var fileName = GenerateExportFileName(cv.Title, request.ExportFormat);
                var contentType = GetContentType(request.ExportFormat);

                _logger.LogInformation($"Successfully exported CV {cv.CvId} for user {request.UserId}");

                return new CVExportResponse
                {
                    Success = true,
                    Message = "CV exported successfully",
                    FileContent = exportContent,
                    FileName = fileName,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exporting CV {request.CvId} for user {request.UserId}");
                return new CVExportResponse
                {
                    Success = false,
                    Message = "An error occurred while exporting the CV"
                };
            }
        }

        public async Task<List<string>> GetSupportedImportFormats()
        {
            return await Task.FromResult(_supportedImportFormats.ToList());
        }

        public async Task<List<string>> GetSupportedExportFormats()
        {
            return await Task.FromResult(_supportedExportFormats.ToList());
        }

        // Private helper methods
        private (bool IsValid, string ErrorMessage) ValidateImportFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "File is required");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_supportedImportFormats.Contains(extension))
                return (false, $"Unsupported file format. Supported formats: {string.Join(", ", _supportedImportFormats)}");

            if (file.Length > 10 * 1024 * 1024) // 10MB limit
                return (false, "File size exceeds 10MB limit");

            return (true, string.Empty);
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];

            return $"{fileName}_{timestamp}_{guid}{extension}";
        }

        private async Task<byte[]> ConvertToFormat(byte[] content, string originalPath, string targetFormat)
        {
            try
            {
                var originalExtension = Path.GetExtension(originalPath).ToLowerInvariant();
                var targetExtension = $".{targetFormat.ToLowerInvariant()}";

                // If same format, return as-is
                if (originalExtension == targetExtension)
                    return content;

                // Use the document parser to convert between formats
                var tempPath = Path.GetTempFileName();
                try
                {
                    await File.WriteAllBytesAsync(tempPath, content);

                    // Parse the document to get plain text
                    var parseResult = await _documentParserService.ParseDocumentAsync(tempPath);

                    // Convert based on target format
                    return targetFormat.ToLowerInvariant() switch
                    {
                        "txt" => Encoding.UTF8.GetBytes(parseResult.PlainTextContent ?? ""),
                        "json" => await ConvertToJson(parseResult),
                        "csv" => await ConvertToCsv(parseResult),
                        _ => content // Return original if conversion not supported
                    };
                }
                finally
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting file format from {Original} to {Target}",
                    Path.GetExtension(originalPath), targetFormat);
                return content; // Return original on error
            }
        }
        public async Task<CvAnalysisResponse> ImportCvAnalysisJsonAsync(CvAnalysisRequest request)
        {
            try
            {
                if (request.File == null || !request.File.FileName.EndsWith(".json"))
                {
                    return new CvAnalysisResponse
                    {
                        Success = false,
                        Message = "Invalid file. Only .json files are allowed."
                    };
                }

                var fileUrl = await _fileStorageService.SaveJsonFileWithOriginalNameAsync(request.File, request.File.FileName);
                var publicId = Path.GetFileNameWithoutExtension(request.File.FileName);

                var analysisFile = new CvAnalysisFile
                {
                    CvVersionId = request.CvVersionId,
                    FileUrl = fileUrl,
                    PublicId = publicId,
                    FileType = "json",
                    Category = "cv_analysis"
                };

                await _unitOfWork.CvAnalysisRepository.CreateAsync(analysisFile);
                await _unitOfWork.SaveChanges();

                return new CvAnalysisResponse
                {
                    Success = true,
                    Message = "JSON file uploaded and saved successfully.",
                    FileUrl = fileUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing CV analysis JSON.");
                return new CvAnalysisResponse
                {
                    Success = false,
                    Message = "An error occurred while importing the JSON analysis."
                };
            }
        }
        public async Task<CvListResponse> GetUserCvsAsync(int userId)
        {
            try
            {
                var cvs = await _unitOfWork.CVRepository.GetByUserIdAsync(userId);

                return new CvListResponse
                {
                    Success = true,
                    Message = $"Found {cvs.Count} CV(s)",
                    Cvs = cvs.Select(cv => new CVDto
                    {
                        CvId = cv.CvId,
                        UserId = cv.UserId,
                        Title = cv.Title,
                        CreatedAt = cv.UploadedAt,
                        UpdatedAt = cv.LastModified,
                        CurrentVersion = cv.CurrentVersion,
                        TotalVersions = cv.CvVersions?.Count ?? 0,
                        FilePath = cv.FilePath,
                        LatestVersion = cv.CvVersions?
                            .OrderByDescending(v => v.VersionNumber)
                            .FirstOrDefault() != null
                            ? new CvVersionDto
                            {
                                VersionId = cv.CvVersions.OrderByDescending(v => v.VersionNumber).First().VersionId,
                                VersionNumber = cv.CvVersions.OrderByDescending(v => v.VersionNumber).First().VersionNumber,
                                FilePath = cv.CvVersions.OrderByDescending(v => v.VersionNumber).First().FilePath,
                                ChangeSummary = cv.CvVersions.OrderByDescending(v => v.VersionNumber).First().ChangeSummary,
                                CreatedAt = cv.CvVersions.OrderByDescending(v => v.VersionNumber).First().CreatedAt
                            }
                            : null
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CVs for user {UserId}", userId);
                return new CvListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving CVs"
                };
            }
        }
        public async Task<CvDetailResponse> GetCvByIdAsync(int cvId)
        {
            try
            {
                var cv = await _unitOfWork.CVRepository.GetByIdAsync(cvId);
                if (cv == null)
                {
                    return new CvDetailResponse
                    {
                        Success = false,
                        Message = "CV not found or access denied"
                    };
                }

                return new CvDetailResponse
                {
                    Success = true,
                    Message = "CV retrieved successfully",
                    Cv = new CvDetailDto
                    {
                        CvId = cv.CvId,
                        UserId = cv.UserId,
                        Title = cv.Title,
                        CreatedAt = cv.UploadedAt,
                        UpdatedAt = cv.LastModified,
                        CurrentVersion = cv.CurrentVersion,
                        FilePath = cv.FilePath,
                        Versions = cv.CvVersions?.Select(v => new CvVersionDto
                        {
                            VersionId = v.VersionId,
                            VersionNumber = v.VersionNumber,
                            FilePath = v.FilePath,
                            ChangeSummary = v.ChangeSummary,
                            CreatedAt = v.CreatedAt
                        }).OrderByDescending(v => v.VersionNumber).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CV {CvId}", cvId);
                return new CvDetailResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the CV"
                };
            }
        }

        public async Task<DeleteResponse> DeleteCvAsync(int userId, int cvId)
        {
            try
            {
                var cv = await _unitOfWork.CVRepository.GetByUserIdAsync(userId, cvId);
                if (cv == null)
                {
                    return new DeleteResponse
                    {
                        Success = false,
                        Message = "CV not found or access denied"
                    };
                }
                if (!string.IsNullOrEmpty(cv.FilePath))
                {
                    var cloudinaryResult = await _fileStorageService.DeleteFileAsync(cv.FilePath);
                    if (!cloudinaryResult)
                    {
                        _logger.LogWarning("Failed to delete CV file from Cloudinary for CV ID {CvId}", cvId);
                    }
                }
                await _unitOfWork.CVRepository.DeleteAsync(cvId);

                return new DeleteResponse
                {
                    Success = true,
                    Message = "CV deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CV {CvId} for user {UserId}", cvId, userId);
                return new DeleteResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the CV"
                };
            }
        }
        private async Task<byte[]> ConvertToJson(ParsedDocumentResult parseResult)
        {
            var jsonData = new
            {
                ExtractedAt = DateTime.UtcNow,
                Sections = parseResult.ExtractedSections,
                PlainText = parseResult.PlainTextContent,
                Warnings = parseResult.Warnings
            };

            var json = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return Encoding.UTF8.GetBytes(json);
        }

        private async Task<byte[]> ConvertToCsv(ParsedDocumentResult parseResult)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Section,Content");

            foreach (var section in parseResult.ExtractedSections)
            {
                var parts = section.Split(':', 2);
                if (parts.Length == 2)
                {
                    var content = parts[1].Replace("\"", "\"\"").Replace("\n", " ").Trim();
                    csv.AppendLine($"\"{parts[0]}\",\"{content}\"");
                }
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string GenerateExportFileName(string? title, string format)
        {
            var fileName = !string.IsNullOrEmpty(title) ? title : "CV";
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
            return $"{fileName}_{timestamp}.{format}";
        }

        private string GetContentType(string format)
        {
            return format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        public async Task<ParsedDocumentResult> ParseCvAsync(int cvId)
        {
            var cv = await _unitOfWork.CVRepository.GetByIdAsync(cvId);
            if (cv == null)
                throw new Exception("CV not found or access denied");

            var fileBytes = await _fileStorageService.GetFileAsync(cv.FilePath); // FilePath is Cloudinary URL or ID
            if (fileBytes == null)
                throw new Exception("CV file not found in storage");

            // Convert byte[] to plain text directly, based on file type
            var plainText = await _documentParserService.ExtractTextAsync(fileBytes, cv.FilePath);

            var result = new ParsedDocumentResult
            {
                PlainTextContent = plainText
            };
            var cvVersion = await _unitOfWork.CVRepository.GetLatestVersionAsync(cvId);
            cvVersion.PlainText = plainText;
            await _unitOfWork.CvVersionRepository.UpdateAsync(cvVersion);
            await _unitOfWork.SaveChanges();
            _documentParserService.ExtractCvSections(result);
            return result;
        }

        public async Task<byte[]> EditPdfAsync(byte[] originalPdf, string newText)
        {
            if (originalPdf == null || originalPdf.Length == 0)
                throw new ArgumentException("Original PDF data cannot be null or empty", nameof(originalPdf));

            if (string.IsNullOrWhiteSpace(newText))
                throw new ArgumentException("New text cannot be null or empty", nameof(newText));

            using var inputStream = new MemoryStream(originalPdf);
            using var outputStream = new MemoryStream();

            try
            {
                using var reader = new PdfReader(inputStream);
                var writerProps = new WriterProperties().SetFullCompressionMode(true);
                using var writer = new PdfWriter(outputStream, writerProps);
                using var pdfDoc = new PdfDocument(reader, writer);

                // Check if PDF has pages
                if (pdfDoc.GetNumberOfPages() == 0)
                    throw new InvalidOperationException("PDF document has no pages");

                var firstPage = pdfDoc.GetFirstPage();
                var pageSize = firstPage.GetPageSize();

                // Create font with error handling
                var font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

                // Use PdfCanvas for more control over text positioning
                var canvas = new PdfCanvas(firstPage);

                // Set text properties
                canvas.BeginText()
                      .SetFontAndSize(font, 12)
                      .SetTextMatrix(50, pageSize.GetHeight() - 50) // Position from top-left
                      .ShowText(newText)
                      .EndText();
                pdfDoc.Close();
                return outputStream.ToArray();
        }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to edit PDF: {ex.Message}", ex);
            }
        }
        public async Task<string?> CategorizeCvByFieldAsync(int cvId, string plainTextContent)
        {
             var fieldId = await _unitOfWork.CVRepository.CategorizeCvByFieldAsync(cvId, plainTextContent);
            var result =  await _unitOfWork.JobFieldRepository.GetJobNameByIdAsync(fieldId);
            return result;
        }
        public Task<ParseCvResponse> ParseCvFromUrlAsync(string fileUrl, bool includeMetadata = true)
        {
            throw new NotImplementedException();
        }
        private int? ExtractMatchScore(string jdAlignment)
        {
            if (string.IsNullOrWhiteSpace(jdAlignment))
                return null;

            var match = Regex.Match(jdAlignment, @"Overall Match Score\*\*:\s*(\d+)", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int score))
            {
                return score;
            }

            return null;
        }
    }
}
