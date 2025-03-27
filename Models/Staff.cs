using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Staff
{
    public long StaffId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public int? Status { get; set; }

    public long StoreId { get; set; }

    public virtual ICollection<MachineIssue> MachineIssues { get; set; } = new List<MachineIssue>();

    public virtual Store Store { get; set; } = null!;
}

public partial class StaffDto
{
    public long StaffId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public int? Status { get; set; }

    public long StoreId { get; set; }

    public virtual ICollection<MachineIssue> MachineIssues { get; set; } = new List<MachineIssue>();
}
