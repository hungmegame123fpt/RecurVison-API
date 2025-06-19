using BusinessObject;
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
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly RecurVisionV1Context _db;
        internal readonly DbSet<T> dbSet;
        public BaseRepository(RecurVisionV1Context db)
        {
            _db = db;
            dbSet = db.Set<T>();
        }
        public async Task<T> CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            //_db.Attach(entity);
            //_db.Entry(entity).State = EntityState.Modified;
            _db.Set<T>().Update(entity);
            //_db.SaveChanges();
            return entity;
        }
        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await dbSet.FindAsync(id) != null;
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();

        }
    }
}
