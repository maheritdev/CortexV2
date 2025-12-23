using Cortex.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cortex.DTOs.Users
{
    public class PatientDto
    {
        public int PatientID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = null!;

        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }

        [StringLength(3, ErrorMessage = "Blood type must be like 'A+', 'O-'")]
        public string? BloodType { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string ContactNumber { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid alternate phone number")]
        public string? AlternateContact { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        [StringLength(100)]
        public string? City { get; set; }

        public string? State { get; set; }

        [RegularExpression(@"^\d{4,10}$", ErrorMessage = "Zip code must be numeric (4–10 digits)")]
        public string? ZipCode { get; set; }

        public string? Country { get; set; }

        public string? EmergencyContactName { get; set; }

        [Phone(ErrorMessage = "Invalid emergency contact number")]
        public string? EmergencyContactNumber { get; set; }

        public string? InsuranceProvider { get; set; }

        public string? InsurancePolicyNumber { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? LastVisitDate { get; set; }

        public string? MedicalHistory { get; set; }

        public string? Allergies { get; set; }

        public string? Status { get; set; }
        public string? Username { get; set; } 
        public string? PasswordHash { get; set; }

        public virtual ICollection<Admission?> Admissions { get; set; } = new List<Admission?>();

        public virtual ICollection<Appointment?> Appointments { get; set; } = new List<Appointment?>();

        public virtual ICollection<Billing?> Billings { get; set; } = new List<Billing?>();

        public virtual ICollection<LabOrder?> LabOrders { get; set; } = new List<LabOrder?>();

        public virtual ICollection<MedicalRecord?> MedicalRecords { get; set; } = new List<MedicalRecord?>();

        public virtual ICollection<Payment?> Payments { get; set; } = new List<Payment?>();

        public virtual ICollection<Prescription?> Prescriptions { get; set; } = new List<Prescription?>();

        public virtual ICollection<User?> User { get; set; } = new List<User?>();

    }
}
