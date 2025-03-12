using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class IssueResolution
{
    public int ResolutionId { get; set; }

    public DateTime ResolutionDate { get; set; }

    public string ResolutionDescription { get; set; } = null!;

    public int Status { get; set; }

    public int IssueId { get; set; }

    public int TechnicianId { get; set; }

    public virtual MachineIssue Issue { get; set; } = null!;

    public virtual Technician Technician { get; set; } = null!;
}
