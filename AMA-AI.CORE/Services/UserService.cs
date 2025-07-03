using AMA_AI.CORE.DTOs.Auth;
using AMA_AI.CORE.DTOs.User;
using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Interfaces.Repositories;
using AMA_AI.CORE.Interfaces.Services;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(RoleEnum role)
        {
            return await _userRepository.GetByRoleAsync(role);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _userRepository.GetActiveUsersAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Validate if username or email already exists
            if (await UsernameExistsAsync(createUserDto.Username))
                throw new InvalidOperationException("Username already exists");

            if (await EmailExistsAsync(createUserDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Role = createUserDto.Role
                // PasswordHash will be set by the calling service (AuthService)
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Update properties
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;
            
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;
            
            if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;
            
            if (!string.IsNullOrEmpty(updateUserDto.Bio))
                user.Bio = updateUserDto.Bio;
            
            if (!string.IsNullOrEmpty(updateUserDto.Address))
                user.Address = updateUserDto.Address;
            
            if (!string.IsNullOrEmpty(updateUserDto.City))
                user.City = updateUserDto.City;
            
            if (!string.IsNullOrEmpty(updateUserDto.Country))
                user.Country = updateUserDto.Country;
            
            if (!string.IsNullOrEmpty(updateUserDto.PostalCode))
                user.PostalCode = updateUserDto.PostalCode;
            
            if (updateUserDto.DateOfBirth.HasValue)
                user.DateOfBirth = updateUserDto.DateOfBirth;

            user.UpdateUser();
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            // Password validation and hashing will be handled by the calling service
            // This method assumes the password has already been validated and hashed
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Token validation and password reset logic will be handled by the calling service
            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            // Token validation and email confirmation logic will be handled by the calling service
            return true;
        }

        public async Task<bool> LockUserAsync(Guid userId, TimeSpan lockoutDuration)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.LockAccount(lockoutDuration);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UnlockUserAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.UnlockAccount();
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> EnableTwoFactorAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.EnableTwoFactor();
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DisableTwoFactorAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.DisableTwoFactor();
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
} 