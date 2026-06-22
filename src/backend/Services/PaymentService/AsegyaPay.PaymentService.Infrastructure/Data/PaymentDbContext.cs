using AsegyaPay.PaymentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsegyaPay.PaymentService.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for the Payment service.
/// Each microservice owns its own database schema.
/// </summary>
public sealed class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Refund> Refunds => Set<Refund>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
