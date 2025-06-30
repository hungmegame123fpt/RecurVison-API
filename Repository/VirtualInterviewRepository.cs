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
    public class VirtualInterviewRepository : IVirtualInterviewRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<VirtualInterview> _dbSet;

        public VirtualInterviewRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<VirtualInterview>();
        }

        public async Task<VirtualInterview> CreateAsync(VirtualInterview entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<VirtualInterview> UpdateAsync(VirtualInterview entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(VirtualInterview entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<VirtualInterview>> GetAllAsync(Expression<Func<VirtualInterview, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<VirtualInterview> query = _dbSet;

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

        public async Task<VirtualInterview> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.InterviewId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Custom methods
        public async Task<List<VirtualInterview>> GetInterviewsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .Include(i => i.Job)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VirtualInterview>> GetInterviewsByJobIdAsync(int jobId)
        {
            return await _dbSet
                .Where(i => i.JobId == jobId)
                .Include(i => i.User)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<VirtualInterview?> GetInterviewWithQuestionsAsync(int interviewId)
        {
            return await _dbSet
                .Include(i => i.InterviewQuestions)
                .Include(i => i.Job)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.InterviewId == interviewId);
        }

        public async Task<List<VirtualInterview>> GetCompletedInterviewsAsync()
        {
            return await _dbSet
                .Where(i => i.Status == "Completed")
                .Include(i => i.InterviewQuestions)
                .ToListAsync();
        }

        public async Task<List<VirtualInterview>> GetInterviewsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(i => i.Status == status)
                .Include(i => i.User)
                .Include(i => i.Job)
                .ToListAsync();
        }

    }
}
