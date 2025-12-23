using Cortex.DTOs.Users;

namespace Cortex.DTOs.auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }

        // one of these will be non-null depending on login type
        public StaffDto? Staff { get; set; }
        public PatientDto? Patient { get; set; }

        // helper
        public string UserType => Staff != null ? "Staff" : "Patient";
    }
    
}
