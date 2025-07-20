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
    public class InterviewQuestionRepository : BaseRepository<InterviewQuestion>, IInterviewQuestionRepository
    {
        public InterviewQuestionRepository(RecurVisionV1Context db) : base(db)
        {
        }

        public async Task<InterviewQuestion> CreateAsync(InterviewQuestion entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<InterviewQuestion> UpdateAsync(InterviewQuestion entity)
        {
            dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(InterviewQuestion entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<List<InterviewQuestion>> GetAllAsync(Expression<Func<InterviewQuestion, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<InterviewQuestion> query = dbSet;

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
            return await dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await dbSet.AnyAsync(x => x.QuestionId == id);
        }
        // Custom methods
        public async Task<List<InterviewQuestion>> GetQuestionsByInterviewIdAsync(int interviewId)
        {
            return await dbSet
                .Where(q => q.InterviewId == interviewId)
                .OrderBy(q => q.QuestionId)
                .ToListAsync();
        }
        public async Task<List<InterviewQuestion>> GetQuestionsBySessionIdAsync(string sessionId)
        {
            var interview =  _db.VirtualInterviews.Where(i => i.SessionId.Equals(sessionId)).FirstOrDefault();
            return await dbSet
                .Where(q => q.InterviewId == interview.InterviewId)
                .OrderBy(q => q.QuestionId)
                .ToListAsync();
        }
        public async Task<InterviewQuestion?> GetQuestionWithAnswerAsync(int questionId)
        {
            return await dbSet
                .Include(q => q.Interview)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<List<InterviewQuestion>> GetUnansweredQuestionsAsync(int interviewId)
        {
            return await dbSet
                .Where(q => q.InterviewId == interviewId && string.IsNullOrEmpty(q.AnswerText))
                .ToListAsync();
        }
    }
}
