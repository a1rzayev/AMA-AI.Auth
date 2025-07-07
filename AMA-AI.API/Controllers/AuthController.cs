using Microsoft.AspNetCore.Mvc;
using AMA_AI.CORE.Interfaces.Services;
using AMA_AI.CORE.DTOs.Auth;
using AMA_AI.CORE.Models;

namespace AMA_AI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, token, user) = await _authService.LoginAsync(loginDto);

            if (!success)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new
            {
                token,
                user = user != null ? new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role
                } : null
            });
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and confirm password do not match" });
            }

            var (success, message, user) = await _authService.RegisterAsync(registerDto);

            if (!success)
            {
                return Conflict(new { message });
            }

            return CreatedAtAction(nameof(Login), new { username = user?.Username }, new
            {
                message,
                user = user != null ? new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                } : null
            });
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var token = await _authService.GeneratePasswordResetTokenAsync(email);

            if (string.IsNullOrEmpty(token))
            {
                return NotFound(new { message = "User with this email not found" });
            }

            // In a real application, you would send this token via email
            return Ok(new { message = "Password reset token generated successfully", token });
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (!success)
            {
                return BadRequest(new { message = "Invalid or expired reset token" });
            }

            return Ok(new { message = "Password reset successfully" });
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user ID from JWT token (you'll need to implement this)
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var success = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (!success)
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var success = await _authService.ConfirmEmailAsync(token);

            if (!success)
            {
                return BadRequest(new { message = "Invalid or expired confirmation token" });
            }

            return Ok(new { message = "Email confirmed successfully" });
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var token = GetTokenFromHeader();
            if (!string.IsNullOrEmpty(token))
            {
                await _authService.LogoutAsync(token);
            }

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var success = await _authService.RefreshTokenAsync(refreshToken);

            if (!success)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            return Ok(new { message = "Token refreshed successfully" });
        }

        [HttpPost("validate-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateToken()
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required" });
            }

            var isValid = await _authService.ValidateTokenAsync(token);

            if (!isValid)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            return Ok(new { message = "Token is valid" });
        }

        #region Helper Methods

        private string GetTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length);
            }
            return string.Empty;
        }

        private Guid GetCurrentUserId()
        {
            // This is a placeholder - you'll need to implement proper JWT token parsing
            // to extract the user ID from the token claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        #endregion
    }
} 