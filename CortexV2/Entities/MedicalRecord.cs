using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class MedicalRecord
{
    public int RecordId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime VisitDate { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Prescription { get; set; }

    public string? Notes { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public int? RecordedBy { get; set; }

    public DateTime? RecordedDate { get; set; }

    public virtual Staff Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual Staff? RecordedByNavigation { get; set; }
    public DateTime CreatedAt { get; internal set; }
}
