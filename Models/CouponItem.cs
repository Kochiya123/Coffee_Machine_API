using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class CouponItem
{
    public int CouponItemId { get; set; }

    public decimal DiscountAmount { get; set; }

    public int Status { get; set; }

    public int CouponId { get; set; }

    public int ProductId { get; set; }

    public virtual Coupon Coupon { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
