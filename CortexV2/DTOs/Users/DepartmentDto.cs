using Cortex.Entities;

namespace Cortex.DTOs.Users
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public string? Description { get; set; }

        public string? Location { get; set; }

        public string? ContactNumber { get; set; }
    }
}
