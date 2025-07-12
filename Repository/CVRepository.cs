using BusinessObject;
using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CVRepository : BaseRepository<Cv>, ICVRepository
    {
        public CVRepository(RecurVisionV1Context db) : base(db)
        {
        }
        public async Task<Cv?> GetByIdAsync(int cvId)
        {
            return await _db.Cvs
                .Include(c => c.CvVersions)
                .FirstOrDefaultAsync(c => c.CvId == cvId);
        } 
        public async Task<List<CVDto>> GetAllAsync()
        {
            return await _db.Cvs
                .Include(c => c.CvVersions)
                .Select(cv => new CVDto
                {
                    CvId = cv.CvId,
                    UserId = cv.UserId,
                    Title = cv.Title,
                    CreatedAt = cv.UploadedAt,
                    UpdatedAt = cv.LastModified,
                    CurrentVersion = cv.CurrentVersion,
                    TotalVersions = cv.CvVersions.Count,
                    FilePath = cv.FilePath,
                    FieldId = cv.FieldId,
                    LatestVersion = cv.CvVersions
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
                }).ToListAsync();


        }

        public async Task<Cv?> GetByUserIdAsync(int userId, int cvId)
        {
            return await _db.Cvs
                .Include(c => c.CvVersions)
                .FirstOrDefaultAsync(c => c.CvId == cvId && c.UserId == userId);
        }

        public async Task<List<Cv>> GetByUserIdAsync(int userId)
        {
            return await _db.Cvs
                .Where(c => c.UserId == userId)
                .Include(c => c.CvVersions)
                .ToListAsync();
        }

        public async Task<Cv> CreateAsync(Cv cv)
        {
            cv.UploadedAt = DateTime.UtcNow;
            cv.LastModified = DateTime.UtcNow;
            cv.Status = "active";
            cv.CurrentVersion = 1;

            _db.Cvs.Add(cv);
            await _db.SaveChangesAsync();
            return cv;
        }

        public async Task<Cv> UpdateAsync(Cv cv)
        {
            cv.LastModified = DateTime.UtcNow;
            _db.Cvs.Update(cv);
            await _db.SaveChangesAsync();
            return cv;
        }

        public async Task<bool> DeleteAsync(int cvId)
        {
            var cv = await _db.Cvs.FindAsync(cvId);
            if (cv == null) return false;

            _db.Cvs.Remove(cv);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int cvId)
        {
            return await _db.Cvs.AnyAsync(c => c.CvId == cvId);
        }

        public async Task<CvVersion?> GetLatestVersionAsync(int cvId)
        {
            return await _db.CvVersions
                .Where(v => v.CvId == cvId)
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<CvVersion> CreateVersionAsync(CvVersion version)
        {
            version.CreatedAt = DateTime.UtcNow;
            _db.CvVersions.Add(version);
            await _db.SaveChangesAsync();
            return version;
        }
        public async Task MatchKeywordsAndSaveAsync(int cvId, string plainTextCv)
        {
            // STEP 1: Normalize keywords from CV
            var cvWords = plainTextCv
                .ToLower()
                .Split(new[] { ' ', '\r', '\n', '\t', '.', ',', ':', ';', '-', '/', '\\', '(', ')', '*', '"' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .Where(w => w.Length > 1)
                .Distinct()
                .ToList();

            if (!cvWords.Any()) return;

            // STEP 2: Get all relevant keywords from DB
            var keywords = await _db.Keywords
                .Include(k => k.JobField)
                .Where(k => cvWords.Contains(k.Keyword1.ToLower()))
                .ToListAsync();

            if (!keywords.Any()) return;

            // STEP 3: Find job postings related to those keyword job fields
            var matchedFieldIds = keywords.Select(k => k.FieldId).Distinct().ToList();

            var jobPostings = await _db.JobPostings
                .Where(j => j.FieldId != null && matchedFieldIds.Contains(j.FieldId.Value))
                .ToListAsync();

            if (!jobPostings.Any()) return;

            var matches = new List<CvKeywordMatch>();

            // STEP 4: Create CvKeywordMatch records
            foreach (var job in jobPostings)
            {
                var relevantKeywords = keywords.Where(k => k.FieldId == job.FieldId);

                foreach (var keyword in relevantKeywords)
                {
                    bool isPresent = cvWords.Contains(keyword.Keyword1.ToLower());

                    matches.Add(new CvKeywordMatch
                    {
                        CvId = cvId,
                        JobId = job.JobId,
                        KeywordId = keyword.KeywordId,
                        IsPresent = isPresent,
                        MatchScore = isPresent ? (keyword.ImportanceScore ?? 1) : 0
                    });
                }
            }

            // STEP 5: Save matches
            if (matches.Any())
            {
                _db.CvKeywordMatches.AddRange(matches);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<int?> CategorizeCvByFieldAsync(int cvId, string plainTextContent)
        {
            var cv = await _db.Cvs.FindAsync(cvId);
            if (cv == null || string.IsNullOrWhiteSpace(plainTextContent))
                return null;

            // Step 1: Extract Objective section
            var objectiveText = ExtractObjectiveSection(plainTextContent).ToLower();

            // Normalize to words
            var objectiveWords = objectiveText
                .Split(new[] { ' ', '\n', '\r', '\t', ',', '.', ':', ';', '-', '/', '\\', '(', ')', '*', '"' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .Where(w => w.Length > 1)
                .Distinct()
                .ToHashSet();

            // Step 2: Match JobField based on Objective
            var allFields = await _db.JobFields.ToListAsync();
            foreach (var field in allFields)
            {
                var fieldWords = field.FieldName.ToLower().Split(' ');
                if (fieldWords.All(w => objectiveWords.Contains(w)))
                {
                    cv.FieldId = field.FieldId;
                    cv.LastModified = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    return field.FieldId;
                }
            }

            // Step 3: Fallback – match by full content using keywords
            var fullWords = plainTextContent
                .ToLower()
                .Split(new[] { ' ', '\n', '\r', '\t', ',', '.', ':', ';', '-', '/', '\\', '(', ')', '*', '"' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .Where(w => w.Length > 1)
                .Distinct()
                .ToHashSet();

            var keywordMatches = await _db.Keywords
                .Where(k => k.FieldId != null && fullWords.Contains(k.Keyword1.ToLower()))
                .ToListAsync();

            if (!keywordMatches.Any()) return null;

            var bestMatch = keywordMatches
                .GroupBy(k => k.FieldId)
                .Select(g => new
                {
                    FieldId = g.Key!.Value,
                    TotalScore = g.Sum(k => k.ImportanceScore ?? 1)
                })
                .OrderByDescending(x => x.TotalScore)
                .FirstOrDefault();

            if (bestMatch == null) return null;

            cv.FieldId = bestMatch.FieldId;
            cv.LastModified = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return bestMatch.FieldId;
        }
        private string ExtractObjectiveSection(string text)
        {
            var start = text.IndexOf("objective", StringComparison.OrdinalIgnoreCase);
            if (start == -1) return string.Empty;

            var end = text.IndexOf("education", start, StringComparison.OrdinalIgnoreCase);
            if (end == -1) end = text.Length;

            return text.Substring(start, end - start).Trim();
        }
    }
}
