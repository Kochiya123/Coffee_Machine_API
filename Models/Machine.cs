using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Machine
{
    public int MachineId { get; set; }

    public string MachineName { get; set; } = null!;

    public DateOnly InstallationDate { get; set; }

    public int Status { get; set; }

    public long StoreId { get; set; }

    public int MachineTypeId { get; set; }

    public virtual ICollection<MachineIssue> MachineIssues { get; set; } = new List<MachineIssue>();

    public virtual ICollection<MachineLog> MachineLogs { get; set; } = new List<MachineLog>();

    public virtual ICollection<MachineProduct> MachineProducts { get; set; } = new List<MachineProduct>();

    public virtual MachineType MachineType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Store Store { get; set; } = null!;
}
