using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Admission
{
    public int AdmissionId { get; set; }

    public int PatientId { get; set; }

    public DateTime AdmissionDate { get; set; }

    public DateTime? DischargeDate { get; set; }

    public int? RoomId { get; set; }

    public int? AssignedDoctorId { get; set; }

    public string? Reason { get; set; }

    public string? Diagnosis { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public string? DischargeSummary { get; set; }

    public virtual Staff? AssignedDoctor { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual Room? Room { get; set; }
}
