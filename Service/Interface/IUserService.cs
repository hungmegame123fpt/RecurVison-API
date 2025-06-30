using BusinessObject.DTO.User;
using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Entities;
using System.Linq.Expressions;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<PaginatedResult<UserDto>> GetUsersAsync(UserFilterDto filter);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> SuspendUserAsync(int userId);
        Task<bool> ActivateUserAsync(int userId);
        Task<UserStatsDto> GetUserStatsAsync();
        Task<List<UserDto>> GetAllAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null);
        //Task<byte[]> ExportUsersAsync(string format = "csv");
    }
}
