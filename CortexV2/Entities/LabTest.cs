using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class LabTest
{
    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public decimal Price { get; set; }

    public string? Duration { get; set; }

    public string? SampleType { get; set; }

    public string? NormalRange { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<LabOrderDetail> LabOrderDetails { get; set; } = new List<LabOrderDetail>();
}
