using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string? CouponCode { get; set; }

    public decimal? DiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? AmountItem { get; set; }

    public int? Status { get; set; }

    public int? PaymentId { get; set; }

    public int? ProductId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Product? Product { get; set; }
}


public partial class CouponDto
{
    public int CouponId { get; set; }

    public string? CouponCode { get; set; }

    public decimal? DiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? AmountItem { get; set; }

    public int? Status { get; set; }

    public int? PaymentId { get; set; }

    public int? ProductId { get; set; }

}


