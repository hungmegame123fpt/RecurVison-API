using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Interface;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Service
{
    public class OtpService : IOTPService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<OtpService> _logger;
        private readonly int _otpExpirationMinutes;
        private readonly int _maxAttempts;
        private readonly int _blockDurationMinutes;

        public OtpService(
            IConnectionMultiplexer redis,
            IConfiguration configuration,
            ILogger<OtpService> logger)
        {
            _redis = redis;
            _db = redis.GetDatabase();
            _logger = logger;
            _otpExpirationMinutes = configuration.GetValue<int>("Redis:OTPExpirationMinutes", 5);
            _maxAttempts = configuration.GetValue<int>("Redis:MaxOTPAttempts", 3);
            _blockDurationMinutes = configuration.GetValue<int>("Redis:BlockDurationMinutes", 30);
        }

        public async Task<string> GenerateOTPAsync(string identifier)
        {
            try
            {
                // Check if user is blocked
                if (await IsUserBlockedAsync(identifier))
                {
                    throw new InvalidOperationException("User is temporarily blocked due to too many failed attempts");
                }

                // Generate 6-digit OTP
                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();

                var otpKey = GetOTPKey(identifier);
                var attemptsKey = GetAttemptsKey(identifier);

                // Store OTP with expiration
                var stored = await StoreOTPAsync(otpKey, otp);

                if (stored)
                {
                    // Reset attempts counter when new OTP is generated
                    await _db.KeyDeleteAsync(attemptsKey);

                    _logger.LogInformation("OTP generated for identifier: {Identifier}", identifier);
                    return otp;
                }

                throw new InvalidOperationException("Failed to store OTP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for identifier: {Identifier}", identifier);
                throw;
            }
        }

        public async Task<bool> VerifyOTPAsync(string identifier, string otp)
        {
            try
            {
                // Check if user is blocked
                if (await IsUserBlockedAsync(identifier))
                {
                    _logger.LogWarning("OTP verification blocked for identifier: {Identifier}", identifier);
                    return false;
                }

                var otpKey = GetOTPKey(identifier);
                var storedOtp = await GetOTPAsync(otpKey);

                if (string.IsNullOrEmpty(storedOtp))
                {
                    _logger.LogWarning("No OTP found for identifier: {Identifier}", identifier);
                    await IncrementFailedAttemptsAsync(identifier);
                    return false;
                }

                if (storedOtp == otp)
                {
                    // OTP is correct, delete it and reset attempts
                    await DeleteOTPAsync(otpKey);
                    await _db.KeyDeleteAsync(GetAttemptsKey(identifier));

                    _logger.LogInformation("OTP verified successfully for identifier: {Identifier}", identifier);
                    return true;
                }

                // OTP is incorrect, increment failed attempts
                await IncrementFailedAttemptsAsync(identifier);
                _logger.LogWarning("Invalid OTP provided for identifier: {Identifier}", identifier);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for identifier: {Identifier}", identifier);
                return false;
            }
        }

        public async Task<bool> StoreOTPAsync(string key, string otp)
        {
            try
            {
                return await _db.StringSetAsync(key, otp, TimeSpan.FromMinutes(_otpExpirationMinutes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing OTP with key: {Key}", key);
                return false;
            }
        }

        public async Task<string?> GetOTPAsync(string key)
        {
            try
            {
                var result = await _db.StringGetAsync(key);
                return result.HasValue ? result.ToString() : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving OTP with key: {Key}", key);
                return null;
            }
        }

        public async Task<bool> DeleteOTPAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting OTP with key: {Key}", key);
                return false;
            }
        }

        public async Task<int> GetRemainingAttemptsAsync(string identifier)
        {
            try
            {
                var attemptsKey = GetAttemptsKey(identifier);
                var attempts = await _db.StringGetAsync(attemptsKey);
                var currentAttempts = attempts.HasValue ? (int)attempts : 0;
                return Math.Max(0, _maxAttempts - currentAttempts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting remaining attempts for identifier: {Identifier}", identifier);
                return 0;
            }
        }

        public async Task<bool> IsUserBlockedAsync(string identifier)
        {
            try
            {
                var blockKey = GetBlockKey(identifier);
                return await _db.KeyExistsAsync(blockKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user is blocked for identifier: {Identifier}", identifier);
                return false;
            }
        }

        private async Task IncrementFailedAttemptsAsync(string identifier)
        {
            try
            {
                var attemptsKey = GetAttemptsKey(identifier);
                var attempts = await _db.StringIncrementAsync(attemptsKey);

                // Set expiration for attempts counter (same as OTP expiration)
                await _db.KeyExpireAsync(attemptsKey, TimeSpan.FromMinutes(_otpExpirationMinutes));

                if (attempts >= _maxAttempts)
                {
                    // Block the user
                    var blockKey = GetBlockKey(identifier);
                    await _db.StringSetAsync(blockKey, "blocked", TimeSpan.FromMinutes(_blockDurationMinutes));

                    _logger.LogWarning("User blocked due to too many failed attempts: {Identifier}", identifier);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing failed attempts for identifier: {Identifier}", identifier);
            }
        }

        private static string GetOTPKey(string identifier) => $"otp:{identifier}";
        private static string GetAttemptsKey(string identifier) => $"otp_attempts:{identifier}";
        private static string GetBlockKey(string identifier) => $"otp_blocked:{identifier}";
    }
}

