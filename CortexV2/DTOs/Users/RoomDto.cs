using Cortex.Entities;

namespace Cortex.DTOs.Users
{
    public class RoomDto
    {
        public int RoomId { get; set; }

        public string RoomNumber { get; set; } = null!;

        public string RoomType { get; set; } = null!;

        public int? DepartmentId { get; set; }

        public int? Capacity { get; set; }

        public string? Status { get; set; }

        public decimal? RatePerDay { get; set; }

        public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();

        public virtual Department? Department { get; set; }
    }
}
