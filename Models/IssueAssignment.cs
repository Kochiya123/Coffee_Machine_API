using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class IssueAssignment
{
    public int AssignmentId { get; set; }

    public DateTime AssignedDate { get; set; }

    public int Status { get; set; }

    public int IssueId { get; set; }

    public int TechnicianId { get; set; }

    public virtual MachineIssue Issue { get; set; } = null!;

    public virtual Technician Technician { get; set; } = null!;
}
