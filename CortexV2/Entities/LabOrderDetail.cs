using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class LabOrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int TestId { get; set; }

    public string? Result { get; set; }

    public DateTime? ResultDate { get; set; }

    public int? PerformedBy { get; set; }

    public int? VerifiedBy { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual LabOrder Order { get; set; } = null!;

    public virtual Staff? PerformedByNavigation { get; set; }

    public virtual LabTest Test { get; set; } = null!;

    public virtual Staff? VerifiedByNavigation { get; set; }
    public DateTime LastUpdated { get; internal set; }
}
