using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public int PaymentStatus { get; set; }

    public int Status { get; set; }

    public int OrderId { get; set; }

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual Order Order { get; set; } = null!;
}
