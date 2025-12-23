using Cortex.DTOs.Users;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class InventoryTransaction
{
    public int TransactionId { get; set; }

    public int? InventoryId { get; set; }
    public int? StaffId { get; set; }

    public string? TransactionType { get; set; }

    public int Quantity { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? Notes { get; set; }

    public int? PerformedBy { get; set; }

    public string? ReferenceNumber { get; set; }

    public virtual Inventory? Inventory { get; set; }
    public virtual Staff? Staff { get; set; }

    public virtual Staff? PerformedByNavigation { get; set; }
}
