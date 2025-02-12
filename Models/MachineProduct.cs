using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class MachineProduct
{
    public int MachineProductId { get; set; }

    public int MachineStockQuantity { get; set; }

    public int Status { get; set; }

    public int MachineId { get; set; }

    public int ProductId { get; set; }

    public virtual Machine Machine { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
