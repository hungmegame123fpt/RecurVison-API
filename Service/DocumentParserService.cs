using BusinessObject.DTO.CV;
using DocumentFormat.OpenXml.Packaging;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DocumentParserService : IDocumentParserService
    {
        private readonly ILogger<DocumentParserService> _logger;

        public DocumentParserService(ILogger<DocumentParserService> logger)
        {
            _logger = logger;
        }

        public async Task<ParsedDocumentResult> ParseDocumentAsync(string filePath, string fileExtension = null)
        {
            var result = new ParsedDocumentResult();
            try
            {
                // Use provided extension or extract from file path
                var extension = fileExtension ?? Path.GetExtension(filePath);

                switch (extension)
                {
                    case ".pdf":
                        await ParsePdfAsync(filePath, result);
                        break;
                    case ".docx":
                        await ParseDocxAsync(filePath, result);
                        break;
                    case ".txt":
                        await ParseTextAsync(filePath, result);
                        break;
                    default:
                        result.Warnings.Add($"Unsupported file format: {extension}");
                        break;
                }

                // Extract CV sections
                if (!string.IsNullOrEmpty(result.PlainTextContent))
                {
                    ExtractCvSections(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing document: {FilePath}", filePath);
                result.Warnings.Add($"Parse error: {ex.Message}");
            }

            return result;
        }
        private async Task ParsePdfAsync(string localFilePath, ParsedDocumentResult result)
        {
            // Your existing PDF parsing logic for local files
            using var reader = new PdfReader(localFilePath);
            using var document = new PdfDocument(reader);

            var strategy = new SimpleTextExtractionStrategy();
            var content = new StringBuilder();

            for (int i = 1; i <= document.GetNumberOfPages(); i++)
            {
                var page = document.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                content.AppendLine(text);
            }

            result.PlainTextContent = content.ToString();
        }

        private async Task ParseDocxAsync(string filePath, ParsedDocumentResult result)
        {
            using var document = WordprocessingDocument.Open(filePath, false);
            var body = document.MainDocumentPart?.Document?.Body;

            if (body != null)
            {
                result.PlainTextContent = body.InnerText;
            }
            else
            {
                result.Warnings.Add("Could not extract content from DOCX file");
            }
        }

        private async Task ParseTextAsync(string filePath, ParsedDocumentResult result)
        {
            result.PlainTextContent = await File.ReadAllTextAsync(filePath);
        }

        private void ExtractCvSections(ParsedDocumentResult result)
        {
            var content = result.PlainTextContent!;
            var sections = new List<string>();

            // Common CV section patterns
            var sectionPatterns = new Dictionary<string, string[]>
            {
                ["Personal Information"] = new[] { "personal", "contact", "profile" },
                ["Summary"] = new[] { "summary", "objective", "profile summary" },
                ["Experience"] = new[] { "experience", "work experience", "employment", "career" },
                ["Education"] = new[] { "education", "academic", "qualifications" },
                ["Skills"] = new[] { "skill", "technical skills", "competencies" },
                ["Projects"] = new[] { "projects", "portfolio" },
                ["Certifications"] = new[] { "certifications", "certificates", "licenses" },
                ["Awards"] = new[] { "awards", "achievements", "honors" },
                ["References"] = new[] { "references" }
            };

            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string currentSection = "";
            var sectionContent = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                // Check if line is a section header
                var detectedSection = DetectSection(trimmedLine, sectionPatterns);

                if (!string.IsNullOrEmpty(detectedSection))
                {
                    // Save previous section
                    if (!string.IsNullOrEmpty(currentSection) && sectionContent.Length > 0)
                    {
                        sections.Add($"{currentSection}:\n{sectionContent.ToString().Trim()}");
                    }

                    currentSection = detectedSection;
                    sectionContent.Clear();
                }
                else if (!string.IsNullOrEmpty(currentSection))
                {
                    sectionContent.AppendLine(trimmedLine);
                }
            }

            // Add the last section
            if (!string.IsNullOrEmpty(currentSection) && sectionContent.Length > 0)
            {
                sections.Add($"{currentSection}:\n{sectionContent.ToString().Trim()}");
            }

            result.ExtractedSections = sections;

            // Add parsing statistics
            if (sections.Count == 0)
            {
                result.Warnings.Add("No CV sections were automatically detected");
            }
        }

        private string DetectSection(string line, Dictionary<string, string[]> patterns)
        {
            var lowerLine = line.ToLowerInvariant();

            foreach (var section in patterns)
            {
                foreach (var keyword in section.Value)
                {
                    if (lowerLine.Contains(keyword) &&
                        (line.EndsWith(':') || line.Length < 50)) // Likely a header
                    {
                        return section.Key;
                    }
                }
            }

            return string.Empty;
        }
    }
}
