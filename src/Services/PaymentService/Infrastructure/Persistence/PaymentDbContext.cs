using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Payment Service.
/// </summary>
public class PaymentDbContext : DbContext
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
    public DbSet<Refund> Refunds => Set<Refund>();

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 4);
            entity.Property(e => e.RefundedAmount).HasPrecision(18, 4);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.Method).HasMaxLength(30);
            entity.Property(e => e.MerchantId).HasMaxLength(50);
            entity.Property(e => e.ReferenceId).HasMaxLength(30);
            entity.Property(e => e.OrderId).HasMaxLength(100);

            entity.HasIndex(e => e.MerchantId);
            entity.HasIndex(e => e.ReferenceId).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.MerchantId, e.CreatedAt });

            entity.HasMany(e => e.Attempts)
                .WithOne()
                .HasForeignKey(a => a.PaymentId);

            entity.HasMany(e => e.Refunds)
                .WithOne()
                .HasForeignKey(r => r.PaymentId);

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<PaymentAttempt>(entity =>
        {
            entity.ToTable("payment_attempts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Method).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(30);
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.ToTable("refunds");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 4);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.ReferenceId).HasMaxLength(30);
            entity.HasIndex(e => e.ReferenceId).IsUnique();
            entity.HasIndex(e => e.PaymentId);
        });
    }
}
