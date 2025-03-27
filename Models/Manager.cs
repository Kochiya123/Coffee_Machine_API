using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Manager
{
    public long ManagerId { get; set; }

    public string Username { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public int Status { get; set; }

    public long? StoreId { get; set; }

    public virtual Store? Store { get; set; }
}

public partial class ManagerDto
{
    public long ManagerId { get; set; }

    public string Username { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public int Status { get; set; }

    public long? StoreId { get; set; }
}
