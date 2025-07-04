using AMA_AI.CORE.Interfaces.Repositories;
using AMA_AI.CORE.Models;

namespace AMA_AI.Infrastructure.Repositories
{
    public class AdminUserRepository : BaseRepository<AdminUser>, IAdminUserRepository
    {
        private readonly Dictionary<string, Guid> _adminCodeToId = new();
        private readonly Dictionary<string, Guid> _usernameToId = new();
        private readonly Dictionary<string, Guid> _emailToId = new();

        public async Task<AdminUser?> GetByAdminCodeAsync(string adminCode)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_adminCodeToId.TryGetValue(adminCode, out var id))
                {
                    return _entities.TryGetValue(id, out var adminUser) ? adminUser : null;
                }
                return null;
            }
        }

        public async Task<IEnumerable<AdminUser>> GetAdminsByDepartmentAsync(string department)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Values.OfType<AdminUser>()
                    .Where(a => !string.IsNullOrEmpty(a.Department) && a.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public async Task<IEnumerable<AdminUser>> GetSuperAdminsAsync()
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Values.OfType<AdminUser>()
                    .Where(a => a.Role == CORE.Enums.RoleEnum.SuperAdmin)
                    .ToList();
            }
        }

        public async Task<bool> AdminCodeExistsAsync(string adminCode)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _adminCodeToId.ContainsKey(adminCode);
            }
        }

        public async Task<AdminUser?> GetByUsernameAsync(string username)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_usernameToId.TryGetValue(username, out var id))
                {
                    return _entities.TryGetValue(id, out var adminUser) ? adminUser : null;
                }
                return null;
            }
        }

        public async Task<AdminUser?> GetByEmailAsync(string email)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_emailToId.TryGetValue(email, out var id))
                {
                    return _entities.TryGetValue(id, out var adminUser) ? adminUser : null;
                }
                return null;
            }
        }

        public override async Task<AdminUser> AddAsync(AdminUser entity)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                _entities[entity.Id] = entity;
                _usernameToId[entity.Username] = entity.Id;
                _emailToId[entity.Email] = entity.Id;
                
                if (!string.IsNullOrEmpty(entity.AdminCode))
                {
                    _adminCodeToId[entity.AdminCode] = entity.Id;
                }
                
                return entity;
            }
        }

        public override async Task<AdminUser> UpdateAsync(AdminUser entity)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_entities.TryGetValue(entity.Id, out var existingAdmin))
                {
                    // Remove old mappings
                    _usernameToId.Remove(existingAdmin.Username);
                    _emailToId.Remove(existingAdmin.Email);
                    if (!string.IsNullOrEmpty(existingAdmin.AdminCode))
                    {
                        _adminCodeToId.Remove(existingAdmin.AdminCode);
                    }

                    // Update entity
                    _entities[entity.Id] = entity;

                    // Add new mappings
                    _usernameToId[entity.Username] = entity.Id;
                    _emailToId[entity.Email] = entity.Id;
                    if (!string.IsNullOrEmpty(entity.AdminCode))
                    {
                        _adminCodeToId[entity.AdminCode] = entity.Id;
                    }
                }
                return entity;
            }
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_entities.TryGetValue(id, out var adminUser))
                {
                    _usernameToId.Remove(adminUser.Username);
                    _emailToId.Remove(adminUser.Email);
                    if (!string.IsNullOrEmpty(adminUser.AdminCode))
                    {
                        _adminCodeToId.Remove(adminUser.AdminCode);
                    }
                    return _entities.Remove(id);
                }
                return false;
            }
        }
    }
} 