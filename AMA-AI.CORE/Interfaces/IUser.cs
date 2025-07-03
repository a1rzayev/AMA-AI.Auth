using AMA_AI.CORE.Enums;

namespace AMA_AI.CORE.Interfaces
{
    public interface IUser
    {
        Guid Id { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        string PasswordHash { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        RoleEnum Role { get; set; }
        bool IsActive { get; set; }
        bool EmailConfirmed { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? LastLoginAt { get; set; }
    }
} 