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
    public class JobFieldRepository : IJobFieldRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<JobField> _dbSet;

        public JobFieldRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<JobField>();
        }

        public async Task<JobField> CreateAsync(JobField entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<JobField> UpdateAsync(JobField entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(JobField entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<JobField>> GetAllAsync(Expression<Func<JobField, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<JobField> query = _dbSet;

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
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.FieldId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<string?> GetJobNameByIdAsync(int? id)
        {
            return await _dbSet
                .Where(k => k.FieldId == id)
                .Select(k => k.FieldName)
                .FirstOrDefaultAsync();
        }
    }
}
