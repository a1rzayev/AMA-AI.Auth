using AMA_AI.CORE.DTOs.Auth;
using AMA_AI.CORE.DTOs.User;
using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Interfaces.Services;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly Dictionary<string, string> _refreshTokens = new(); // In-memory storage for demo

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<(bool success, string token, User? user)> LoginAsync(LoginDto loginDto)
        {
            // Get user by username or email
            var user = await _userService.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                // Try email
                user = await _userService.GetByEmailAsync(loginDto.Username);
            }

            if (user == null)
                return (false, "Invalid username or email", null);

            // Check if user is active and not locked
            if (!user.IsActive)
                return (false, "Account is deactivated", null);

            if (user.IsLocked)
                return (false, "Account is locked", null);

            if (!user.EmailConfirmed)
                return (false, "Email not confirmed", null);

            // Validate password (in a real implementation, you would hash and compare)
            if (!ValidatePassword(loginDto.Password, user.PasswordHash))
            {
                // Increment login attempts
                user.IncrementLoginAttempts();
                await _userService.UpdateAsync(user);

                // Lock account if too many attempts
                if (user.LoginAttempts >= 5)
                {
                    await _userService.LockUserAsync(user.Id, TimeSpan.FromMinutes(30));
                    return (false, "Too many failed attempts. Account locked for 30 minutes.", null);
                }

                return (false, "Invalid password", null);
            }

            // Reset login attempts on successful login
            user.ResetLoginAttempts();
            user.UpdateLastLogin();
            await _userService.UpdateAsync(user);

            // Generate JWT token (simplified for demo)
            var token = GenerateJwtToken(user);
            return (true, token, user);
        }

        public async Task<(bool success, string message, User? user)> RegisterAsync(RegisterDto registerDto)
        {
            // Validate passwords match
            if (registerDto.Password != registerDto.ConfirmPassword)
                return (false, "Passwords do not match", null);

            // Check if username or email already exists
            if (await _userService.UsernameExistsAsync(registerDto.Username))
                return (false, "Username already exists", null);

            if (await _userService.EmailExistsAsync(registerDto.Email))
                return (false, "Email already exists", null);

            // Create user DTO
            var createUserDto = new CreateUserDto
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = RoleEnum.User
            };

            // Create user
            var user = await _userService.CreateUserAsync(createUserDto);

            // Hash password and update user
            user.PasswordHash = HashPassword(registerDto.Password);
            await _userService.UpdateAsync(user);

            return (true, "User registered successfully", user);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            // In a real implementation, you would validate the JWT token
            // For demo purposes, we'll just check if it's not empty
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return string.Empty;

            // Generate a simple token (in real implementation, use proper token generation)
            var token = Guid.NewGuid().ToString("N");
            
            // Store token (in real implementation, store in database with expiration)
            _refreshTokens[token] = user.Id.ToString();

            return token;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Validate passwords match
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmNewPassword)
                return false;

            // Validate token (in real implementation, check token validity and expiration)
            if (!_refreshTokens.TryGetValue(resetPasswordDto.Token, out var userIdStr))
                return false;

            if (!Guid.TryParse(userIdStr, out var userId))
                return false;

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Update password
            user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
            await _userService.UpdateAsync(user);

            // Remove used token
            _refreshTokens.Remove(resetPasswordDto.Token);

            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            // Validate passwords match
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                return false;

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Validate current password
            if (!ValidatePassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            // Update password
            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            await _userService.UpdateAsync(user);

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            // In a real implementation, you would validate the email confirmation token
            // For demo purposes, we'll just return true
            return true;
        }

        public async Task<bool> LogoutAsync(string token)
        {
            // In a real implementation, you would invalidate the token
            // For demo purposes, we'll just return true
            return true;
        }

        public async Task<bool> RefreshTokenAsync(string refreshToken)
        {
            // In a real implementation, you would validate and refresh the token
            // For demo purposes, we'll just return true
            return true;
        }

        private bool ValidatePassword(string password, string passwordHash)
        {
            // In a real implementation, you would use BCrypt or similar
            // For demo purposes, we'll just check if password is not empty
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }

        private string HashPassword(string password)
        {
            // In a real implementation, you would use BCrypt or similar
            // For demo purposes, we'll just return a simple hash
            return $"hashed_{password}";
        }

        private string GenerateJwtToken(User user)
        {
            // In a real implementation, you would generate a proper JWT token
            // For demo purposes, we'll just return a simple token
            return $"jwt_token_{user.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
} 