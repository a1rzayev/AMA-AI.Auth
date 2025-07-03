using AMA_AI.CORE.DTOs.User;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Interfaces.Services
{
    public interface IAdminUserService : IBaseService<AdminUser>
    {
        Task<AdminUser?> GetByAdminCodeAsync(string adminCode);
        Task<IEnumerable<AdminUser>> GetAdminsByDepartmentAsync(string department);
        Task<IEnumerable<AdminUser>> GetSuperAdminsAsync();
        Task<bool> AdminCodeExistsAsync(string adminCode);
        Task<AdminUser> GrantAdminPrivilegesAsync(Guid userId, string grantedBy);
        Task<AdminUser> GrantSuperAdminPrivilegesAsync(Guid userId, string grantedBy);
        Task<bool> RevokeAdminPrivilegesAsync(Guid userId);
        Task<bool> UpdateAdminPermissionsAsync(Guid userId, bool canManageUsers, bool canManageRoles, bool canViewLogs);
    }
} 