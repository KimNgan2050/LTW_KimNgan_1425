namespace BTT3_KimNgan.Models
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Age { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
