using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public int? Capacity { get; set; }

    public string? Status { get; set; }

    public decimal? RatePerDay { get; set; }

    public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();

    public virtual Department? Department { get; set; }
}
