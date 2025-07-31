using BusinessObject;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class JobFieldRepository : BaseRepository<JobField>, IJobFieldRepository
    {
        public JobFieldRepository(RecurVisionV1Context db) : base(db)
        {
        }

        public async Task<JobField> CreateAsync(JobField entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<JobField> UpdateAsync(JobField entity)
        {
            dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(JobField entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<List<JobField>> GetAllAsync(Expression<Func<JobField, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<JobField> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<JobField> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await dbSet.AnyAsync(x => x.FieldId == id);
        }
        public async Task<string?> GetJobNameByIdAsync(int? id)
        {
            return await dbSet
                .Where(k => k.FieldId == id)
                .Select(k => k.FieldName)
                .FirstOrDefaultAsync();
        }
        public async Task<int?> GetMatchingFieldIdAsync(string jobTitle)
        {
            if (string.IsNullOrWhiteSpace(jobTitle)) return null;

            var jobFields = await GetAllAsync(f => f.IsActive && !string.IsNullOrEmpty(f.TypicalKeywords));

            jobTitle = jobTitle.ToLower();
            int maxMatch = 0;
            JobField? bestMatch = null;

            foreach (var field in jobFields)
            {
                var keywords = field.TypicalKeywords!.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(k => k.Trim().ToLower());

                int matchCount = keywords.Count(k => jobTitle.Contains(k));
                if (matchCount > maxMatch)
                {
                    maxMatch = matchCount;
                    bestMatch = field;
                }
            }

            return bestMatch?.FieldId;
        }
    }
}
