using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class MachineLog
{
    public int LogId { get; set; }

    public DateTime LogDate { get; set; }

    public string LogDescription { get; set; } = null!;

    public int LogType { get; set; }

    public long PerformedBy { get; set; }

    public int Status { get; set; }

    public int MachineId { get; set; }

    public int TechnicianId { get; set; }

    public virtual Machine Machine { get; set; } = null!;

    public virtual Technician Technician { get; set; } = null!;
}
