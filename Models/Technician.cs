using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Technician
{
    public int TechnicianId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<IssueAssignment> IssueAssignments { get; set; } = new List<IssueAssignment>();

    public virtual ICollection<IssueResolution> IssueResolutions { get; set; } = new List<IssueResolution>();

    public virtual ICollection<MachineLog> MachineLogs { get; set; } = new List<MachineLog>();
}

public partial class TechnicianDto
{
    public int TechnicianId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<IssueAssignment> IssueAssignments { get; set; } = new List<IssueAssignment>();

    public virtual ICollection<IssueResolution> IssueResolutions { get; set; } = new List<IssueResolution>();

    public virtual ICollection<MachineLog> MachineLogs { get; set; } = new List<MachineLog>();
}
