using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? OrderDescription { get; set; }

    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    public long CustomerId { get; set; }

    public int MachineId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Machine Machine { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}



public class OrderDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? OrderDescription { get; set; }

    public decimal TotalAmount { get; set; }

    public long CustomerId { get; set; }

    public int MachineId { get; set; }

    public int Status { get; set; }
}

public class OrderCustomerDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? OrderDescription { get; set; }

    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; }

}