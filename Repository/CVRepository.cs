using BusinessObject;
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

    }
}
