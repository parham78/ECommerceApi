using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class OrderManagementDbContext
    : IdentityDbContext<ApplicationUser>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public OrderManagementDbContext(
        DbContextOptions<OrderManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product price
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
    .Property(p => p.RowVersion)
    .IsRowVersion();
        modelBuilder.Entity<Product>()
        .Property(p => p.Sku)
        .HasMaxLength(50)
        .IsRequired();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();
        modelBuilder.Entity<Customer>()
        .HasIndex(c => c.Email)
        .IsUnique();
        modelBuilder.Entity<Customer>()
    .HasOne(c => c.User)
    .WithOne()
    .HasForeignKey<Customer>(c => c.UserId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.CustomerId);

        // Order total price
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
    .Property(o => o.Status)
    .HasConversion<string>();

        // OrderItem historical price
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        // Customer 1 ---- many Orders
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order 1 ---- many OrderItems
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product 1 ---- many OrderItems
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}