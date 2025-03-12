using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class MachineIssue
{
    public int IssueId { get; set; }

    public DateTime ReportDate { get; set; }

    public string IssueDescription { get; set; } = null!;

    public int Status { get; set; }

    public int MachineId { get; set; }

    public long ReportedBy { get; set; }

    public virtual ICollection<IssueAssignment> IssueAssignments { get; set; } = new List<IssueAssignment>();

    public virtual ICollection<IssueResolution> IssueResolutions { get; set; } = new List<IssueResolution>();

    public virtual Machine Machine { get; set; } = null!;

    public virtual Staff ReportedByNavigation { get; set; } = null!;
}
