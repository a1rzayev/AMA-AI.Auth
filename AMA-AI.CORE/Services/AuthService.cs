using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AMA_AI.CORE.DTOs.Auth;
using AMA_AI.CORE.DTOs.User;
using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Interfaces.Services;
using AMA_AI.CORE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AMA_AI.CORE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly Dictionary<string, string> _refreshTokens = new(); // In-memory storage for demo

        public AuthService(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        public async Task<(bool success, string token, User? user)> LoginAsync(LoginDto loginDto)
        {
            Console.WriteLine($"Login attempt for: {loginDto.Username}");
            Console.WriteLine($"RememberMe: {loginDto.RememberMe}");
            
            var user = await _userService.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                user = await _userService.GetByEmailAsync(loginDto.Username);
                Console.WriteLine($"User by email: {user != null}");
            }
            Console.WriteLine($"User found: {user != null}");
            if (user != null)
            {
                Console.WriteLine($"Password validation attempt");
                Console.WriteLine($"Input password: {loginDto.Password}");
                Console.WriteLine($"Stored hash: {user.PasswordHash}");
        
                bool passwordValid = ValidatePassword(loginDto.Password, user.PasswordHash);
                Console.WriteLine($"Password valid: {passwordValid}");
            }

            if (!ValidatePassword(loginDto.Password, user.PasswordHash))
            {
                user.IncrementLoginAttempts();
                await _userService.UpdateAsync(user);

                if (user.LoginAttempts >= 5)
                {
                    await _userService.LockUserAsync(user.Id, TimeSpan.FromMinutes(30));
                    return (false, "Too many failed attempts. Account locked for 30 minutes.", null);
                }

                return (false, "Invalid password", null);
            }

            user.ResetLoginAttempts();
            user.UpdateLastLogin();
            await _userService.UpdateAsync(user);

            var token = GenerateJwtToken(user);
            return (true, token, user);
        }

        public async Task<(bool success, string message, User? user)> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
                return (false, "Passwords do not match", null);

            if (await _userService.UsernameExistsAsync(registerDto.Username))
                return (false, "Username already exists", null);

            if (await _userService.EmailExistsAsync(registerDto.Email))
                return (false, "Email already exists", null);

            var createUserDto = new CreateUserDto
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                Role = RoleEnum.User
            };

            var user = await _userService.CreateUserAsync(createUserDto);

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
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 