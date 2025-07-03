namespace AMA_AI.CORE.DTOs.User
{
    public class AdminUserDto : UserDto
    {
        public string? AdminCode { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanManageRoles { get; set; }
        public bool CanViewLogs { get; set; }
        public DateTime? AdminSince { get; set; }
        public string? AssignedBy { get; set; }
        public bool CanManageAdmins { get; set; }
    }
} 