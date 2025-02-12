using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class MachineType
{
    public int MachineTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? MachineDescription { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
