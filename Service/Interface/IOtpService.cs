using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IOTPService
    {
        Task<string> GenerateOTPAsync(string identifier);
        Task<bool> VerifyOTPAsync(string identifier, string otp);
        Task<bool> StoreOTPAsync(string key, string otp);
        Task<string?> GetOTPAsync(string key);
        Task<bool> DeleteOTPAsync(string key);
        Task<int> GetRemainingAttemptsAsync(string identifier);
        Task<bool> IsUserBlockedAsync(string identifier);
    }
}
