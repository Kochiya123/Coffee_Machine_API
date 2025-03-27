using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public string? Path { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public int? Status { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual ICollection<MachineProduct> MachineProducts { get; set; } = new List<MachineProduct>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}


public class ProductDto
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;

    public string? Path { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 10000000, ErrorMessage = "Price must be between 0.01 and 10,000,000.")]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int? StockQuantity { get; set; }

    [Required]
    [Range(0, 1, ErrorMessage = "Status must be 0 (inactive) or 1 (active).")]
    public int? Status { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [FromForm] // ✅ Image file from form-data
    public IFormFile? Image { get; set; }
}