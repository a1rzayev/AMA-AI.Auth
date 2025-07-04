using AMA_AI.CORE.DTOs.Auth;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(bool success, string token, User? user)> LoginAsync(LoginDto loginDto);
        Task<(bool success, string message, User? user)> RegisterAsync(RegisterDto registerDto);
        Task<bool> ValidateTokenAsync(string token);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<bool> ConfirmEmailAsync(string token);
        Task<bool> LogoutAsync(string token);
        Task<bool> RefreshTokenAsync(string refreshToken);
    }
} 