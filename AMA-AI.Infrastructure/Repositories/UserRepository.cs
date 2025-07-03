using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Interfaces.Repositories;
using AMA_AI.CORE.Models;

namespace AMA_AI.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly Dictionary<string, Guid> _usernameToId = new();
        private readonly Dictionary<string, Guid> _emailToId = new();

        public async Task<User?> GetByUsernameAsync(string username)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_usernameToId.TryGetValue(username, out var id))
                {
                    return _entities.TryGetValue(id, out var user) ? user : null;
                }
                return null;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_emailToId.TryGetValue(email, out var id))
                {
                    return _entities.TryGetValue(id, out var user) ? user : null;
                }
                return null;
            }
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(RoleEnum role)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Values.OfType<User>().Where(u => u.Role == role).ToList();
            }
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Values.OfType<User>().Where(u => u.IsActive).ToList();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _usernameToId.ContainsKey(username);
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _emailToId.ContainsKey(email);
            }
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                // Try username first
                if (_usernameToId.TryGetValue(usernameOrEmail, out var id))
                {
                    return _entities.TryGetValue(id, out var user) ? user : null;
                }

                // Try email
                if (_emailToId.TryGetValue(usernameOrEmail, out id))
                {
                    return _entities.TryGetValue(id, out var user) ? user : null;
                }

                return null;
            }
        }

        public override async Task<User> AddAsync(User entity)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                _entities[entity.Id] = entity;
                _usernameToId[entity.Username] = entity.Id;
                _emailToId[entity.Email] = entity.Id;
                return entity;
            }
        }

        public override async Task<User> UpdateAsync(User entity)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_entities.TryGetValue(entity.Id, out var existingUser))
                {
                    // Remove old username/email mappings
                    _usernameToId.Remove(existingUser.Username);
                    _emailToId.Remove(existingUser.Email);

                    // Update entity
                    _entities[entity.Id] = entity;

                    // Add new username/email mappings
                    _usernameToId[entity.Username] = entity.Id;
                    _emailToId[entity.Email] = entity.Id;
                }
                return entity;
            }
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                if (_entities.TryGetValue(id, out var user))
                {
                    _usernameToId.Remove(user.Username);
                    _emailToId.Remove(user.Email);
                    return _entities.Remove(id);
                }
                return false;
            }
        }
    }
} 