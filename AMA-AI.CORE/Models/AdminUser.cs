using AMA_AI.CORE.Enums;

namespace AMA_AI.CORE.Models
{
    public class AdminUser : BaseUser
    {
        // Admin-specific properties
        public string? AdminCode { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public bool CanManageUsers { get; set; } = true;
        public bool CanManageRoles { get; set; } = false;
        public bool CanViewLogs { get; set; } = true;
        public DateTime? AdminSince { get; set; }
        public string? AssignedBy { get; set; }

        // Admin-specific computed properties
        public new bool IsSuperAdmin => Role == RoleEnum.SuperAdmin;
        public bool CanManageAdmins { get; set; } = false;

        // Admin-specific methods
        public virtual void GrantAdminPrivileges(string grantedBy)
        {
            Role = RoleEnum.Admin;
            AdminSince = DateTime.UtcNow;
            AssignedBy = grantedBy;
            UpdateUser();
        }

        public virtual void GrantSuperAdminPrivileges(string grantedBy)
        {
            Role = RoleEnum.SuperAdmin;
            AdminSince = DateTime.UtcNow;
            AssignedBy = grantedBy;
            CanManageRoles = true;
            CanManageAdmins = true;
            UpdateUser();
        }

        public virtual void RevokeAdminPrivileges()
        {
            Role = RoleEnum.User;
            AdminSince = null;
            AssignedBy = null;
            CanManageUsers = false;
            CanManageRoles = false;
            CanViewLogs = false;
            CanManageAdmins = false;
            UpdateUser();
        }

        public override void UpdateLastLogin()
        {
            base.UpdateLastLogin();
            // Additional admin-specific login logic can be added here
        }
    }
} 