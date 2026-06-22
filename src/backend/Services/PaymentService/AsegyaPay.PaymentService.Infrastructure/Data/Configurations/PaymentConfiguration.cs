using AsegyaPay.PaymentService.Domain.Entities;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsegyaPay.PaymentService.Infrastructure.Data.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(p => p.OrderId)
            .HasColumnName("order_id")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.MerchantId)
            .HasColumnName("merchant_id")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(64);

        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(p => p.CapturedAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("captured_amount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("captured_currency").HasMaxLength(3);
        });

        builder.OwnsOne(p => p.RefundedAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("refunded_amount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("refunded_currency").HasMaxLength(3);
        });

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.Method)
            .HasColumnName("method")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.Gateway)
            .HasColumnName("gateway")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.GatewayTransactionId)
            .HasColumnName("gateway_transaction_id")
            .HasMaxLength(128);

        builder.Property(p => p.GatewayOrderId)
            .HasColumnName("gateway_order_id")
            .HasMaxLength(128);

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(512);

        builder.Property(p => p.CallbackUrl)
            .HasColumnName("callback_url")
            .HasMaxLength(2048);

        builder.Property(p => p.RedirectUrl)
            .HasColumnName("redirect_url")
            .HasMaxLength(2048);

        builder.Property(p => p.FraudScore)
            .HasColumnName("fraud_score")
            .HasDefaultValue(0);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(p => p.AuthorizedAt)
            .HasColumnName("authorized_at");

        builder.Property(p => p.CapturedAt)
            .HasColumnName("captured_at");

        builder.Property(p => p.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(p => p.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb");

        builder.HasMany(p => p.Refunds)
            .WithOne()
            .HasForeignKey(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.OrderId).HasDatabaseName("ix_payments_order_id");
        builder.HasIndex(p => p.MerchantId).HasDatabaseName("ix_payments_merchant_id");
        builder.HasIndex(p => p.GatewayTransactionId).HasDatabaseName("ix_payments_gateway_tx_id");
        builder.HasIndex(p => p.CreatedAt).HasDatabaseName("ix_payments_created_at");
        builder.HasIndex(p => new { p.MerchantId, p.Status }).HasDatabaseName("ix_payments_merchant_status");

        builder.Ignore(p => p.DomainEvents);
    }
}
