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
    public class KeywordRepository : BaseRepository<Keyword>, IKeywordRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Keyword> _dbSet;

        public KeywordRepository(RecurVisionV1Context db) : base(db)
        {
        }
        public async Task<IEnumerable<Keyword>> GetAllAsync(string? search = null)
        {
            IQueryable<Keyword> query = _dbSet.Include(k => k.JobField);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                query = query.Where(k => k.Keyword1.ToLower().Contains(lowered) ||
                                         k.Category!.ToLower().Contains(lowered) ||
                                         (k.JobField != null && k.JobField.FieldName.ToLower().Contains(lowered)));
            }

            return await query.ToListAsync();
        }

        public async Task<Keyword> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.KeywordId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<string?> GetKeywordNameByIdAsync(int? id)
        {
            return await _dbSet
                .Where(k => k.KeywordId == id)
                .Select(k => k.Keyword1)
                .FirstOrDefaultAsync();
        }
    }
}
