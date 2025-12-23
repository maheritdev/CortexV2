using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Category { get; set; }

    public string? Description { get; set; }

    public int? QuantityInStock { get; set; }

    public string? UnitOfMeasure { get; set; }

    public int? ReorderLevel { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? Supplier { get; set; }

    public DateTime? LastRestockedDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    public DateTime LastUpdated { get; internal set; }
}
