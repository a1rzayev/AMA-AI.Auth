using AMA_AI.CORE.Enums;

namespace AMA_AI.CORE.Models
{
    public class User : BaseUser
    {
        // Additional properties specific to User
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool TwoFactorEnabled { get; set; } = false;
        public int LoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        // Additional computed properties
        public bool IsLocked => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;
        public bool IsEligibleForLogin => IsActive && !IsLocked && EmailConfirmed;

        // Additional methods
        public virtual void IncrementLoginAttempts()
        {
            LoginAttempts++;
            UpdateUser();
        }

        public virtual void ResetLoginAttempts()
        {
            LoginAttempts = 0;
            UpdateUser();
        }

        public virtual void LockAccount(TimeSpan lockoutDuration)
        {
            LockoutEnd = DateTime.UtcNow.Add(lockoutDuration);
            UpdateUser();
        }

        public virtual void UnlockAccount()
        {
            LockoutEnd = null;
            LoginAttempts = 0;
            UpdateUser();
        }

        public virtual void EnableTwoFactor()
        {
            TwoFactorEnabled = true;
            UpdateUser();
        }

        public virtual void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
            UpdateUser();
        }

        public override void UpdateLastLogin()
        {
            base.UpdateLastLogin();
            ResetLoginAttempts();
        }
    }
} 