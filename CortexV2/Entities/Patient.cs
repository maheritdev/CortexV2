using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cortex.Entities;

public partial class Patient
{
    public int PatientId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? BloodType { get; set; }

    public string ContactNumber { get; set; } = null!;

    public string? AlternateContact { get; set; }

    public string? Email { get; set; }

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }

    public string? Country { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public string? InsuranceProvider { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public DateTime? LastVisitDate { get; set; }

    public string? MedicalHistory { get; set; }

    public string? Allergies { get; set; }

    public string? Status { get; set; }
   
    // Authentication fields (added by us)
    [MaxLength(50)]
    public string? Username { get; set; }

    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<User> User { get; set; } = new List<User>();
}
