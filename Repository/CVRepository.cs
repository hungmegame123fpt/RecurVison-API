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
    public class CVRepository : ICVRepository
    {
        private readonly RecurVisionV1Context _context;

        public CVRepository(RecurVisionV1Context context)
        {
            _context = context;
        }
        public async Task<Cv?> GetByIdAsync(int cvId)
        {
            return await _context.Cvs
                .Include(c => c.CvVersions)
                .FirstOrDefaultAsync(c => c.CvId == cvId);
        } 
        public async Task<List<CVDto>> GetAllAsync()
        {
            return await _context.Cvs
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
            return await _context.Cvs
                .Include(c => c.CvVersions)
                .FirstOrDefaultAsync(c => c.CvId == cvId && c.UserId == userId);
        }

        public async Task<List<Cv>> GetByUserIdAsync(int userId)
        {
            return await _context.Cvs
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

            _context.Cvs.Add(cv);
            await _context.SaveChangesAsync();
            return cv;
        }

        public async Task<Cv> UpdateAsync(Cv cv)
        {
            cv.LastModified = DateTime.UtcNow;
            _context.Cvs.Update(cv);
            await _context.SaveChangesAsync();
            return cv;
        }

        public async Task<bool> DeleteAsync(int cvId)
        {
            var cv = await _context.Cvs.FindAsync(cvId);
            if (cv == null) return false;

            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int cvId)
        {
            return await _context.Cvs.AnyAsync(c => c.CvId == cvId);
        }

        public async Task<CvVersion?> GetLatestVersionAsync(int cvId)
        {
            return await _context.CvVersions
                .Where(v => v.CvId == cvId)
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<CvVersion> CreateVersionAsync(CvVersion version)
        {
            version.CreatedAt = DateTime.UtcNow;
            _context.CvVersions.Add(version);
            await _context.SaveChangesAsync();
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
            var keywords = await _context.Keywords
                .Include(k => k.JobField)
                .Where(k => cvWords.Contains(k.Keyword1.ToLower()))
                .ToListAsync();

            if (!keywords.Any()) return;

            // STEP 3: Find job postings related to those keyword job fields
            var matchedFieldIds = keywords.Select(k => k.FieldId).Distinct().ToList();

            var jobPostings = await _context.JobPostings
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
                _context.CvKeywordMatches.AddRange(matches);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int?> CategorizeCvByFieldAsync(int cvId, string plainTextContent)
        {
            var cv = await _context.Cvs.FindAsync(cvId);
            if (cv == null || string.IsNullOrWhiteSpace(plainTextContent))
                return null;

            // Step 1: Normalize text
            var words = plainTextContent
                .ToLower()
                .Split(new[] { ' ', '\n', '\r', '\t', ',', '.', ':', ';', '-', '/', '\\', '(', ')', '*', '"' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .Where(w => w.Length > 1)
                .Distinct()
                .ToList();

            // Step 2: Load keywords with associated FieldId
            var keywordMatches = await _context.Keywords
                .Where(k => k.FieldId != null && words.Contains(k.Keyword1.ToLower()))
                .ToListAsync();

            if (!keywordMatches.Any()) return null;

            // Step 3: Group by field_id and calculate match score (sum of importance)
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

            // Step 4: Update the Cv with selected FieldId
            cv.FieldId = bestMatch.FieldId;
            cv.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return bestMatch.FieldId;
        }

    }
}
