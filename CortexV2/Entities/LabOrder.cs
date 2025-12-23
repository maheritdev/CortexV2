using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class    LabOrder
{
    public int OrderId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual Staff Doctor { get; set; } = null!;

    public virtual ICollection<LabOrderDetail> LabOrderDetails { get; set; } = new List<LabOrderDetail>();

    public virtual Patient Patient { get; set; } = null!;
}
