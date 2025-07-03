using AMA_AI.CORE.DTOs.Auth;
using AMA_AI.CORE.DTOs.User;
using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Interfaces.Services
{
    public interface IUserService : IBaseService<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(RoleEnum role);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<User> CreateUserAsync(CreateUserDto createUserDto);
        Task<User> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ConfirmEmailAsync(string token);
        Task<bool> LockUserAsync(Guid userId, TimeSpan lockoutDuration);
        Task<bool> UnlockUserAsync(Guid userId);
        Task<bool> EnableTwoFactorAsync(Guid userId);
        Task<bool> DisableTwoFactorAsync(Guid userId);
    }
} 