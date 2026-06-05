using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BTT3_KimNgan.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        // ĐÃ SỬA: Gán gía trị mặc định rỗng để dập tắt cảnh báo CS8618
        public string FullName { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Age { get; set; }
    }
}