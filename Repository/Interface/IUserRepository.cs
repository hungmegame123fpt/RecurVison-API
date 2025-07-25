﻿using BusinessObject.DTO;
using BusinessObject.DTO.User;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<int> CancelUsersNotLoggedInSinceAsync(DateTime cutoffDate);
        Task<List<User>> GetAllAsync(bool includeRelations = false);
        Task<JobPosting?> GetLatestJobPostingAsync(int userId);
        Task<List<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null);
        Task<User?> GetByIdAsync(int? id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByCvIdAsync(int cvId);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<PaginatedResult<User>> GetUsersWithPaginationAsync(UserFilterDto filter);
        Task<UserStatsDto> GetUserStatsAsync();
        Task<List<User>> GetNewSignupsAsync(DateTime fromDate);
        Task<List<User>> GetAllUsersWithSubscriptionsAsync();
    }
}
