using AMA_AI.CORE.DTOs.User;
using AMA_AI.CORE.Interfaces.Repositories;
using AMA_AI.CORE.Interfaces.Services;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Services
{
    public class AdminUserService : BaseService<AdminUser>, IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IUserRepository _userRepository;

        public AdminUserService(IAdminUserRepository adminUserRepository, IUserRepository userRepository) 
            : base(adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
            _userRepository = userRepository;
        }

        public async Task<AdminUser?> GetByAdminCodeAsync(string adminCode)
        {
            return await _adminUserRepository.GetByAdminCodeAsync(adminCode);
        }

        public async Task<IEnumerable<AdminUser>> GetAdminsByDepartmentAsync(string department)
        {
            return await _adminUserRepository.GetAdminsByDepartmentAsync(department);
        }

        public async Task<IEnumerable<AdminUser>> GetSuperAdminsAsync()
        {
            return await _adminUserRepository.GetSuperAdminsAsync();
        }

        public async Task<bool> AdminCodeExistsAsync(string adminCode)
        {
            return await _adminUserRepository.AdminCodeExistsAsync(adminCode);
        }

        public async Task<AdminUser> GrantAdminPrivilegesAsync(Guid userId, string grantedBy)
        {
            // First, get the user from the regular user repository
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Check if user is already an admin
            var existingAdmin = await _adminUserRepository.GetByUsernameAsync(user.Username);
            if (existingAdmin != null)
                throw new InvalidOperationException("User is already an admin");

            // Create new admin user
            var adminUser = new AdminUser
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                AdminCode = GenerateAdminCode(),
                CanManageUsers = true,
                CanViewLogs = true
            };

            adminUser.GrantAdminPrivileges(grantedBy);
            return await _adminUserRepository.AddAsync(adminUser);
        }

        public async Task<AdminUser> GrantSuperAdminPrivilegesAsync(Guid userId, string grantedBy)
        {
            // First, get the user from the regular user repository
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Check if user is already an admin
            var existingAdmin = await _adminUserRepository.GetByUsernameAsync(user.Username);
            if (existingAdmin != null)
            {
                // Update existing admin to super admin
                existingAdmin.GrantSuperAdminPrivileges(grantedBy);
                return await _adminUserRepository.UpdateAsync(existingAdmin);
            }

            // Create new super admin user
            var adminUser = new AdminUser
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                AdminCode = GenerateAdminCode(),
                CanManageUsers = true,
                CanManageRoles = true,
                CanViewLogs = true
            };

            adminUser.GrantSuperAdminPrivileges(grantedBy);
            return await _adminUserRepository.AddAsync(adminUser);
        }

        public async Task<bool> RevokeAdminPrivilegesAsync(Guid userId)
        {
            var adminUser = await GetByIdAsync(userId);
            if (adminUser == null)
                return false;

            adminUser.RevokeAdminPrivileges();
            await _adminUserRepository.UpdateAsync(adminUser);
            return true;
        }

        public async Task<bool> UpdateAdminPermissionsAsync(Guid userId, bool canManageUsers, bool canManageRoles, bool canViewLogs)
        {
            var adminUser = await GetByIdAsync(userId);
            if (adminUser == null)
                return false;

            adminUser.CanManageUsers = canManageUsers;
            adminUser.CanManageRoles = canManageRoles;
            adminUser.CanViewLogs = canViewLogs;
            adminUser.UpdateUser();

            await _adminUserRepository.UpdateAsync(adminUser);
            return true;
        }

        private string GenerateAdminCode()
        {
            // Generate a unique admin code (you might want to implement a more sophisticated algorithm)
            return $"ADM{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
        }
    }
} 