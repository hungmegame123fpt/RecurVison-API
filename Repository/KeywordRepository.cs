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
    public class KeywordRepository : IKeywordRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Keyword> _dbSet;

        public KeywordRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<Keyword>();
        }

        public async Task<Keyword> CreateAsync(Keyword entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<Keyword> UpdateAsync(Keyword entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(Keyword entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<Keyword>> GetAllAsync(Expression<Func<Keyword, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Keyword> query = _dbSet;

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
