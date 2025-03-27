using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Customer
{
    public long CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
public partial class CustomerDto
{
    public long CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}

public partial class AuthenticationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string FirebaseUserId { get; set; }
}

