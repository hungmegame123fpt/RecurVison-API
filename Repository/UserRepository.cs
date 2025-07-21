using BusinessObject;
using BusinessObject.DTO.User;
using BusinessObject.DTO;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(RecurVisionV1Context db) : base(db)
        {
        }
        public async Task<User?> GetByIdAsync(int? id)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<List<User>> GetAllAsync(bool includeRelations = false)
        {
            IQueryable<User> query = _db.Users;

            if (includeRelations)
            {
                query = query
                    .Include(u => u.CareerPlans)
                    .Include(u => u.Cvs)
                    .Include(u => u.JobPostings)
                    .Include(u => u.UserCharacteristics)
                    .Include(u => u.UserRoleAssignedByNavigations)
                    .Include(u => u.UserRoleUsers)
                    .Include(u => u.UserSubscriptions)
                    .Include(u => u.VirtualInterviews)
                    .Include(u => u.UserFieldPreferences);
            }

            return await query.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.AccountStatus == "Active");
        }
        public async Task<User?> GetByCvIdAsync(int cvId)
        {
            var cv = await _db.Cvs.FirstOrDefaultAsync(c => c.CvId == cvId);
            if (cv == null || string.IsNullOrWhiteSpace(cv.UserId.ToString()))
                return null;

            return await _db.Users.FirstOrDefaultAsync(u => u.UserId == cv.UserId && u.AccountStatus == "Active");
        }
        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId && u.AccountStatus == "Active");
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }
        public async Task<List<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<User> query = _db.Users;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.ToListAsync();
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<PaginatedResult<User>> GetUsersWithPaginationAsync(UserFilterDto filter)
        {
            var query = _db.Users.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search) ||
                                       u.Email.Contains(filter.Search) ||
                                       u.FirstName.Contains(filter.Search) ||
                                       u.LastName.Contains(filter.Search) || u.AccountStatus.Contains(filter.Search));
            }
            if (filter.CreatedFrom.HasValue)
            {
                query = query.Where(u => u.CreatedAt >= filter.CreatedFrom.Value);
            }

            if (filter.CreatedTo.HasValue)
            {
                query = query.Where(u => u.CreatedAt <= filter.CreatedTo.Value);
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResult<User>
            {
                Data = users,
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages
            };
        }
        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            return new UserStatsDto
            {
                TotalUsers = await _db.Users.CountAsync(),
                ActiveUsers = await _db.Users.CountAsync(u => u.AccountStatus.Equals("Active")),
                SuspendedUsers = await _db.Users.CountAsync(u => u.AccountStatus.Equals("Suspended")),
                NewSignupsToday = await _db.Users.CountAsync(u => u.CreatedAt == today),
                NewSignupsThisWeek = await _db.Users.CountAsync(u => u.CreatedAt >= weekStart),
                NewSignupsThisMonth = await _db.Users.CountAsync(u => u.CreatedAt >= monthStart)
            };
        }
        public async Task<List<User>> GetNewSignupsAsync(DateTime fromDate)
        {
            return await _db.Users
                .Where(u => u.CreatedAt >= fromDate)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }
        public async Task<int> CancelUsersNotLoggedInSinceAsync(DateTime cutoffDate)
        {
            var usersToCancel = await _db.Users
                .Where(u => u.LastLogin != null && u.LastLogin < cutoffDate && u.AccountStatus != "Cancelled")
                .ToListAsync();

            foreach (var user in usersToCancel)
            {
                user.AccountStatus = "Cancelled";
            }

            await _db.SaveChangesAsync();
            return usersToCancel.Count;
        }
        public async Task<List<User>> GetAllUsersWithSubscriptionsAsync()
        {
            return await _db.Users
                .Include(u => u.UserSubscriptions.Where(s => s.PaymentStatus == "ACTIVE"))
                .ThenInclude(s => s.Plan)
                .ToListAsync();
        }
    }
}
