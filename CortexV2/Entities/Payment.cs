using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? BillId { get; set; }

    public int PatientId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionReference { get; set; }

    public string? Notes { get; set; }

    public int? ReceivedBy { get; set; }

    public virtual Billing? Bill { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual Staff? ReceivedByNavigation { get; set; }
}
