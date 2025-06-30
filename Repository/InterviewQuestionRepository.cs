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
    public class InterviewQuestionRepository : IInterviewQuestionRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<InterviewQuestion> _dbSet;

        public InterviewQuestionRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<InterviewQuestion>();
        }

        public async Task<InterviewQuestion> CreateAsync(InterviewQuestion entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<InterviewQuestion> UpdateAsync(InterviewQuestion entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(InterviewQuestion entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<InterviewQuestion>> GetAllAsync(Expression<Func<InterviewQuestion, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<InterviewQuestion> query = _dbSet;

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

        public async Task<InterviewQuestion> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.QuestionId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Custom methods
        public async Task<List<InterviewQuestion>> GetQuestionsByInterviewIdAsync(int interviewId)
        {
            return await _dbSet
                .Where(q => q.InterviewId == interviewId)
                .OrderBy(q => q.QuestionId)
                .ToListAsync();
        }

        public async Task<InterviewQuestion?> GetQuestionWithAnswerAsync(int questionId)
        {
            return await _dbSet
                .Include(q => q.Interview)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<List<InterviewQuestion>> GetUnansweredQuestionsAsync(int interviewId)
        {
            return await _dbSet
                .Where(q => q.InterviewId == interviewId && string.IsNullOrEmpty(q.AnswerText))
                .ToListAsync();
        }
    }
}
