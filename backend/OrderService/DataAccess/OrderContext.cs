using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.DataAccess;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithOne(o => o.Invocie)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
