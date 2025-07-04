using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Models;

namespace AMA_AI.CORE.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(RoleEnum role);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    }
} 