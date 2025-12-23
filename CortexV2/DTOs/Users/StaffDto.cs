using Cortex.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cortex.DTOs.Users
{
    public class StaffDto
    {
        public int StaffID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public int? DepartmentID { get; set; }
        public string? Position { get; set; }
        public string? Specialization { get; set; }
        public string? Role { get; set; }

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Address { get; set; }
        public string? Qualification { get; set; }

        public DateOnly HireDate { get; set; }

        public decimal? Salary { get; set; }

        public string? Status { get; set; }

        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();

        public virtual ICollection<Appointment> AppointmentCreatedByNavigations { get; set; } = new List<Appointment>();

        public virtual ICollection<Appointment> AppointmentDoctors { get; set; } = new List<Appointment>();

        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

        public virtual Department? Department { get; set; }

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

        public virtual ICollection<LabOrderDetail> LabOrderDetailPerformedByNavigations { get; set; } = new List<LabOrderDetail>();

        public virtual ICollection<LabOrderDetail> LabOrderDetailVerifiedByNavigations { get; set; } = new List<LabOrderDetail>();

        public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

        public virtual ICollection<MedicalRecord> MedicalRecordDoctors { get; set; } = new List<MedicalRecord>();

        public virtual ICollection<MedicalRecord> MedicalRecordRecordedByNavigations { get; set; } = new List<MedicalRecord>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
