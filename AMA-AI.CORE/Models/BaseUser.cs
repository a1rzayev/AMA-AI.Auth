using AMA_AI.CORE.Enums;
using AMA_AI.CORE.Interfaces;

namespace AMA_AI.CORE.Models
{
    public abstract class BaseUser : IUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public RoleEnum Role { get; set; } = RoleEnum.User;
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Computed properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsAdmin => Role == RoleEnum.Admin || Role == RoleEnum.SuperAdmin;
        public bool IsSuperAdmin => Role == RoleEnum.SuperAdmin;

        // Virtual methods for extensibility
        public virtual void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public virtual void UpdateUser()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            UpdateUser();
        }

        public virtual void Activate()
        {
            IsActive = true;
            UpdateUser();
        }

        public virtual void ConfirmEmail()
        {
            EmailConfirmed = true;
            UpdateUser();
        }
    }
} 