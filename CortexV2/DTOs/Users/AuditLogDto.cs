using Cortex.Entities;

namespace Cortex.DTOs.Users
{
    public class AuditLogDto
    {
        public int LogId { get; set; }

        public string TableName { get; set; } = null!;

        public string RecordId { get; set; } = null!;

        public string Action { get; set; } = null!;

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public int? ChangedBy { get; set; }

        public DateTime? ChangeDate { get; set; }

        public string? Ipaddress { get; set; }

        public virtual Staff? ChangedByNavigation { get; set; }
    }
}
