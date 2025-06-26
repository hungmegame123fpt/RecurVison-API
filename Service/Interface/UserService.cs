using AutoMapper;
using BusinessObject.DTO.User;
using BusinessObject.DTO;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Service.Interface
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PaginatedResult<UserDto>> GetUsersAsync(UserFilterDto filter)
        {
            var result = await _userRepository.GetUsersWithPaginationAsync(filter);

            return new PaginatedResult<UserDto>
            {
                Data = _mapper.Map<List<UserDto>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }
        public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            // Check for duplicate email/username
            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
                if (existingUser != null && existingUser.UserId != userId)
                {
                    throw new ValidationException("Email already exists");
                }
            }

            _mapper.Map(updateDto, user);
            var updatedUser = await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} updated successfully", userId);
            return _mapper.Map<UserDto>(updatedUser);
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Soft delete - deactivate instead of hard delete
            user.AccountStatus = "Banned";
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation($"User {user.LastName} deactivated successfully", userId);
            return true;
        }
        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.AccountStatus = "Activated";
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} activated successfully", userId);
            return true;
        }
        public async Task<bool> SuspendUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.AccountStatus = "Suspended";
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation($"User {user.LastName} suspended successfully", userId);
            return true;
        }

        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            return await _userRepository.GetUserStatsAsync();
        }

        //public async Task<byte[]> ExportUsersAsync(string format = "csv")
        //{
        //    var users = await _userRepository.GetAllAsync();
        //    var userDtos = _mapper.Map<List<UserDto>>(users);

        //    if (format.ToLower() == "csv")
        //    {
        //        //return GenerateCsvFile(userDtos);
        //    }
        //    else
        //    {
        //        //return GenerateExcelFile(userDtos);
        //    }
        //}
    }
}
