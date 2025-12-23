using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Cortex.Entities
{
    public class User
    {
        [Key] public int UserId { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string PasswordHash { get; set; }
        [Required] public UserRole Role { get; set; }

        // Common info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Navigation
        public Patient PatientProfile { get; set; }
        public Staff StaffProfile { get; set; }
    }
}
