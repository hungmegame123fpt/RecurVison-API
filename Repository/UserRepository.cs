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
    public class UserRepository : IUserRepository
    {
        private readonly RecurVisionV1Context _context;

        public UserRepository(RecurVisionV1Context context)
        {
            _context = context;
        }
        public async Task<User?> GetByIdAsync(int? id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<List<User>> GetAllAsync(bool includeRelations = false)
        {
            IQueryable<User> query = _context.Users;

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
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.AccountStatus == "Active");
        }

        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId && u.AccountStatus == "Active");
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<List<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<User> query = _context.Users;

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
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<PaginatedResult<User>> GetUsersWithPaginationAsync(UserFilterDto filter)
        {
            var query = _context.Users.AsQueryable();

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
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.AccountStatus.Equals("Active")),
                SuspendedUsers = await _context.Users.CountAsync(u => u.AccountStatus.Equals("Suspended")),
                NewSignupsToday = await _context.Users.CountAsync(u => u.CreatedAt == today),
                NewSignupsThisWeek = await _context.Users.CountAsync(u => u.CreatedAt >= weekStart),
                NewSignupsThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= monthStart)
            };
        }
        public async Task<List<User>> GetNewSignupsAsync(DateTime fromDate)
        {
            return await _context.Users
                .Where(u => u.CreatedAt >= fromDate)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }
        public async Task<int> CancelUsersNotLoggedInSinceAsync(DateTime cutoffDate)
        {
            var usersToCancel = await _context.Users
                .Where(u => u.LastLogin != null && u.LastLogin < cutoffDate && u.AccountStatus != "Cancelled")
                .ToListAsync();

            foreach (var user in usersToCancel)
            {
                user.AccountStatus = "Cancelled";
            }

            await _context.SaveChangesAsync();
            return usersToCancel.Count;
        }
    }
}
