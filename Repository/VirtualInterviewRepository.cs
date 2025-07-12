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
    public class VirtualInterviewRepository : BaseRepository<VirtualInterview>, IVirtualInterviewRepository
    {
        public VirtualInterviewRepository(RecurVisionV1Context db) : base(db)
        {
        }

        public async Task<VirtualInterview> CreateAsync(VirtualInterview entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<VirtualInterview> UpdateAsync(VirtualInterview entity)
        {
            dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(VirtualInterview entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<List<VirtualInterview>> GetAllAsync(Expression<Func<VirtualInterview, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<VirtualInterview> query = dbSet;

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
            return await dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await dbSet.AnyAsync(x => x.InterviewId == id);
        }

        // Custom methods
        public async Task<List<VirtualInterview>> GetInterviewsByUserIdAsync(int userId)
        {
            return await dbSet
                .Where(i => i.UserId == userId)
                .Include(i => i.Job)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VirtualInterview>> GetInterviewsByJobIdAsync(int jobId)
        {
            return await dbSet
                .Where(i => i.JobId == jobId)
                .Include(i => i.User)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<VirtualInterview?> GetInterviewWithQuestionsAsync(int interviewId)
        {
            return await dbSet
                .Include(i => i.InterviewQuestions)
                .Include(i => i.Job)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.InterviewId == interviewId);
        }

        public async Task<List<VirtualInterview>> GetCompletedInterviewsAsync()
        {
            return await dbSet
                .Where(i => i.Status == "Completed")
                .Include(i => i.InterviewQuestions)
                .ToListAsync();
        }

        public async Task<List<VirtualInterview>> GetInterviewsByStatusAsync(string status)
        {
            return await dbSet
                .Where(i => i.Status == status)
                .Include(i => i.User)
                .Include(i => i.Job)
                .ToListAsync();
        }

    }
}
