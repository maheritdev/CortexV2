using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class PrescriptionDetail
{
    public int PrescriptionDetailId { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicationId { get; set; }

    public string Dosage { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string? Instructions { get; set; }

    public int Quantity { get; set; }

    public virtual Medication Medication { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
