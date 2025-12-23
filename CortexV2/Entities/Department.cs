using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Location { get; set; }

    public string? ContactNumber { get; set; }

    public int? HeadOfDepartment { get; set; }

    public virtual Staff? HeadOfDepartmentNavigation { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
