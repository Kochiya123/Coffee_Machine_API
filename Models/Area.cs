using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
