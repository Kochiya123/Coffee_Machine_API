using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Store
{
    public long StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public string StoreLocation { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int Status { get; set; }

    public int AreaId { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
