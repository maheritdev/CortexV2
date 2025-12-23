using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public virtual Staff? CreatedByNavigation { get; set; }

    public virtual Staff Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public DateTime StartTime { get; internal set; }
    public DateTime EndTime { get; internal set; }
}
