using BusinessObject.DTO;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<APIResponse<User>> RegisterAsync(RegisterDTO registerDto);
        Task<APIResponse<User>> LoginAsync(LoginDTO loginDto);
        Task<APIResponse<User>> GoogleAuthAsync(AuthDTO googleAuthDto);
        Task<APIResponse<bool>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<APIResponse<string>> GenerateOtpAsync(string email);
        Task<APIResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDto);
        Task<APIResponse<User?>> GetUserAsync(int userId);
    }
}
