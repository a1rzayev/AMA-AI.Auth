using AMA_AI.CORE.Interfaces.Repositories;

namespace AMA_AI.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly Dictionary<Guid, T> _entities = new();
        protected readonly object _lock = new();

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.TryGetValue(id, out var entity) ? entity : null;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Values.ToList();
            }
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                // Get the Id property using reflection
                var idProperty = entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var id = (Guid)idProperty.GetValue(entity)!;
                    _entities[id] = entity;
                }
                return entity;
            }
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                var idProperty = entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var id = (Guid)idProperty.GetValue(entity)!;
                    if (_entities.ContainsKey(id))
                    {
                        _entities[id] = entity;
                    }
                }
                return entity;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Remove(id);
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.ContainsKey(id);
            }
        }

        public virtual async Task<int> CountAsync()
        {
            await Task.Delay(1); // Simulate async operation
            lock (_lock)
            {
                return _entities.Count;
            }
        }
    }
} 