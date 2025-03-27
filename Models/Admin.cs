using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Admin
{
    public long AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public int Status { get; set; }
}
