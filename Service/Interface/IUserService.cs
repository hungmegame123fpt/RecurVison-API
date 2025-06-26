using BusinessObject.DTO.User;
using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //Task<byte[]> ExportUsersAsync(string format = "csv");
    }
}
