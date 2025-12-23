using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Medication
{
    public int MedicationId { get; set; }

    public string Name { get; set; } = null!;

    public string? GenericName { get; set; }

    public string? Manufacturer { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? UnitType { get; set; }

    public decimal UnitPrice { get; set; }

    public int? ReorderLevel { get; set; }

    public int? CurrentStock { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
