using AMA_AI.CORE.Interfaces.Repositories;
using AMA_AI.CORE.Models;
using AMA_AI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AMA_AI.Infrastructure.Repositories
{
    public class AdminUserRepository : BaseRepository<AdminUser>, IAdminUserRepository
    {
        public AdminUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AdminUser?> GetByAdminCodeAsync(string adminCode)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.AdminCode == adminCode);
        }

        public async Task<IEnumerable<AdminUser>> GetAdminsByDepartmentAsync(string department)
        {
            return await _dbSet.Where(a => 
                !string.IsNullOrEmpty(a.Department) && 
                a.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<IEnumerable<AdminUser>> GetSuperAdminsAsync()
        {
            return await _dbSet.Where(a => a.Role == CORE.Enums.RoleEnum.SuperAdmin)
                .ToListAsync();
        }

        public async Task<bool> AdminCodeExistsAsync(string adminCode)
        {
            return await _dbSet.AnyAsync(a => a.AdminCode == adminCode);
        }

        public async Task<AdminUser?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<AdminUser?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
        }
    }
} 