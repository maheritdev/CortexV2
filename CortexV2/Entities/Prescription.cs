using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex.Entities;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int? RecordId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime PrescriptionDate { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }
    public int Quantity { get; set; }

    [ForeignKey("Medication")] public int MedicationId { get; set; }

    public virtual Staff Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();

    public virtual MedicalRecord? Record { get; set; }
    public Medication Medication { get; set; }
    public DateTime IssuedAt { get; internal set; }
}
