using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

public partial class CoffeeShop01Context : DbContext
{
    public CoffeeShop01Context()
    {
    }

    public CoffeeShop01Context(DbContextOptions<CoffeeShop01Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<IssueAssignment> IssueAssignments { get; set; }

    public virtual DbSet<IssueResolution> IssueResolutions { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<MachineIssue> MachineIssues { get; set; }

    public virtual DbSet<MachineLog> MachineLogs { get; set; }

    public virtual DbSet<MachineProduct> MachineProducts { get; set; }

    public virtual DbSet<MachineType> MachineTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Technician> Technicians { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-MODFQ7SR\\SQLEXPRESS;Database=CoffeeShop_01;User ID=sa;Password=123456;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Area");

            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.AreaName).HasMaxLength(255);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("Coupon");

            entity.Property(e => e.CouponId).HasColumnName("CouponID");
            entity.Property(e => e.CouponCode).HasMaxLength(50);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Payment).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Coupon_Payment");

            entity.HasOne(d => d.Product).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Coupon_Product");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IssueAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.ToTable("IssueAssignment");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.AssignedDate).HasColumnType("datetime");
            entity.Property(e => e.IssueId).HasColumnName("IssueID");
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");

            entity.HasOne(d => d.Issue).WithMany(p => p.IssueAssignments)
                .HasForeignKey(d => d.IssueId)
                .HasConstraintName("FK_IssueAssignment_MachineIssue");

            entity.HasOne(d => d.Technician).WithMany(p => p.IssueAssignments)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IssueAssignment_Technician");
        });

        modelBuilder.Entity<IssueResolution>(entity =>
        {
            entity.HasKey(e => e.ResolutionId);

            entity.ToTable("IssueResolution");

            entity.Property(e => e.ResolutionId).HasColumnName("ResolutionID");
            entity.Property(e => e.IssueId).HasColumnName("IssueID");
            entity.Property(e => e.ResolutionDate).HasColumnType("datetime");
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");

            entity.HasOne(d => d.Issue).WithMany(p => p.IssueResolutions)
                .HasForeignKey(d => d.IssueId)
                .HasConstraintName("FK_IssueResolution_Issue");

            entity.HasOne(d => d.Technician).WithMany(p => p.IssueResolutions)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IssueResolution_Technician");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.ToTable("Machine");

            entity.Property(e => e.MachineId).HasColumnName("MachineID");
            entity.Property(e => e.MachineName).HasMaxLength(255);
            entity.Property(e => e.MachineTypeId).HasColumnName("MachineTypeID");
            entity.Property(e => e.StoreId).HasColumnName("StoreID");

            entity.HasOne(d => d.MachineType).WithMany(p => p.Machines)
                .HasForeignKey(d => d.MachineTypeId)
                .HasConstraintName("FK_Machine_MachineType");

            entity.HasOne(d => d.Store).WithMany(p => p.Machines)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("FK_Machine_Store");
        });

        modelBuilder.Entity<MachineIssue>(entity =>
        {
            entity.HasKey(e => e.IssueId);

            entity.ToTable("MachineIssue");

            entity.Property(e => e.IssueId).HasColumnName("IssueID");
            entity.Property(e => e.MachineId).HasColumnName("MachineID");
            entity.Property(e => e.ReportDate).HasColumnType("datetime");

            entity.HasOne(d => d.Machine).WithMany(p => p.MachineIssues)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("FK_MachineIssue_Machine");

            entity.HasOne(d => d.ReportedByNavigation).WithMany(p => p.MachineIssues)
                .HasForeignKey(d => d.ReportedBy)
                .HasConstraintName("FK_MachineIssue_ReportedBy");
        });

        modelBuilder.Entity<MachineLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("MachineLog");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.MachineId).HasColumnName("MachineID");
            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");

            entity.HasOne(d => d.Machine).WithMany(p => p.MachineLogs)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("FK_IssueResolution_Machine");

            entity.HasOne(d => d.Technician).WithMany(p => p.MachineLogs)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MachineLog_Technician");
        });

        modelBuilder.Entity<MachineProduct>(entity =>
        {
            entity.ToTable("MachineProduct");

            entity.Property(e => e.MachineProductId)
                .ValueGeneratedNever()
                .HasColumnName("MachineProductID");
            entity.Property(e => e.MachineId).HasColumnName("MachineID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Machine).WithMany(p => p.MachineProducts)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("FK_MachineProduct_Machine");

            entity.HasOne(d => d.Product).WithMany(p => p.MachineProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_MachineProduct_Product");
        });

        modelBuilder.Entity<MachineType>(entity =>
        {
            entity.ToTable("MachineType");

            entity.Property(e => e.MachineTypeId).HasColumnName("MachineTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.MachineId).HasColumnName("MachineID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Order_Customer");

            entity.HasOne(d => d.Machine).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("FK_Order_Machine");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderDetail_Product");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Payment_Order");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.StoreId).HasColumnName("StoreID");

            entity.HasOne(d => d.Store).WithMany(p => p.Staff)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_Store");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Store");

            entity.Property(e => e.StoreId).HasColumnName("StoreID");
            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StoreLocation).HasMaxLength(255);
            entity.Property(e => e.StoreName).HasMaxLength(255);

            entity.HasOne(d => d.Area).WithMany(p => p.Stores)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK_Store_Area");
        });

        modelBuilder.Entity<Technician>(entity =>
        {
            entity.ToTable("Technician");

            entity.Property(e => e.TechnicianId).HasColumnName("TechnicianID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.TransactionAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.WalletId).HasColumnName("WalletID");

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Order");

            entity.HasOne(d => d.Payment).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Payment");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK_Transaction_Wallet");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallet");

            entity.Property(e => e.WalletId).HasColumnName("WalletID");
            entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Wallet_Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
