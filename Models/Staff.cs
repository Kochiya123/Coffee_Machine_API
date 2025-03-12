using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Staff
{
    public long StaffId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Status { get; set; }

    public long StoreId { get; set; }

    public virtual ICollection<MachineIssue> MachineIssues { get; set; } = new List<MachineIssue>();

    public virtual Store Store { get; set; } = null!;
}
