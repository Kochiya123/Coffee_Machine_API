using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Store
{
    public long StoreId { get; set; }

    public string? StoreName { get; set; }

    public string? StoreLocation { get; set; }

    public string? PhoneNumber { get; set; }

    public int Status { get; set; }

    public int? AreaId { get; set; }

    public virtual Area? Area { get; set; }

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}

public partial class StoreDto
{
    public long StoreId { get; set; }

    public string? StoreName { get; set; }

    public string? StoreLocation { get; set; }

    public string? PhoneNumber { get; set; }

    public int Status { get; set; }

    public int? AreaId { get; set; }

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
