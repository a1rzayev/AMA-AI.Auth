using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Interfaces.Repositories
{
    public interface IAdminUserRepository : IBaseRepository<AdminUser>
    {
        Task<AdminUser?> GetByAdminCodeAsync(string adminCode);
        Task<IEnumerable<AdminUser>> GetAdminsByDepartmentAsync(string department);
        Task<IEnumerable<AdminUser>> GetSuperAdminsAsync();
        Task<bool> AdminCodeExistsAsync(string adminCode);
        Task<AdminUser?> GetByUsernameAsync(string username);
        Task<AdminUser?> GetByEmailAsync(string email);
    }
} 