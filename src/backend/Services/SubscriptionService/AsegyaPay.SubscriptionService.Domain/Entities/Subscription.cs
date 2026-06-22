using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.SubscriptionService.Domain.Entities;

/// <summary>
/// Represents a subscription plan (e.g., Monthly Pro, Annual Enterprise).
/// </summary>
public sealed class Plan : AggregateRoot<Guid>
{
    public string MerchantId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public BillingInterval Interval { get; private set; }
    public int IntervalCount { get; private set; }
    public int? TrialPeriodDays { get; private set; }
    public bool IsUsageBased { get; private set; }
    public string? UsageType { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Plan() : base(Guid.NewGuid()) { }

    public static Plan Create(
        string merchantId,
        string name,
        decimal amount,
        string currency,
        BillingInterval interval,
        int intervalCount = 1,
        int? trialDays = null,
        string? description = null)
    {
        return new Plan
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Name = name,
            Amount = amount,
            Currency = currency,
            Interval = interval,
            IntervalCount = intervalCount,
            TrialPeriodDays = trialDays,
            IsActive = true,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Represents a customer's active subscription to a plan.
/// </summary>
public sealed class Subscription : AggregateRoot<Guid>
{
    private readonly List<SubscriptionInvoice> _invoices = [];

    public string MerchantId { get; private set; }
    public string CustomerId { get; private set; }
    public Guid PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CurrentPeriodStart { get; private set; }
    public DateTime CurrentPeriodEnd { get; private set; }
    public DateTime? TrialStart { get; private set; }
    public DateTime? TrialEnd { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? CancelAtPeriodEnd { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? NextRetryAt { get; private set; }
    public string? PaymentMethodToken { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<SubscriptionInvoice> Invoices => _invoices.AsReadOnly();

    private Subscription() : base(Guid.NewGuid()) { }

    public static Subscription Create(
        string merchantId,
        string customerId,
        Guid planId,
        string paymentMethodToken,
        int? trialDays = null)
    {
        var now = DateTime.UtcNow;
        DateTime trialEnd = trialDays.HasValue ? now.AddDays(trialDays.Value) : now;
        bool hasTrial = trialDays.HasValue && trialDays > 0;

        return new Subscription
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            CustomerId = customerId,
            PlanId = planId,
            Status = hasTrial ? SubscriptionStatus.Trialing : SubscriptionStatus.Active,
            CurrentPeriodStart = hasTrial ? trialEnd : now,
            CurrentPeriodEnd = hasTrial ? trialEnd.AddMonths(1) : now.AddMonths(1),
            TrialStart = hasTrial ? now : null,
            TrialEnd = hasTrial ? trialEnd : null,
            PaymentMethodToken = paymentMethodToken,
            RetryCount = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Cancel(bool immediately = false)
    {
        if (immediately)
        {
            Status = SubscriptionStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
        }
        else
        {
            CancelAtPeriodEnd = CurrentPeriodEnd;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void Renew(DateTime newPeriodEnd)
    {
        CurrentPeriodStart = CurrentPeriodEnd;
        CurrentPeriodEnd = newPeriodEnd;
        Status = SubscriptionStatus.Active;
        RetryCount = 0;
        NextRetryAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkPaymentFailed(int maxRetries = 3)
    {
        RetryCount++;
        if (RetryCount >= maxRetries)
        {
            Status = SubscriptionStatus.PastDue;
        }
        NextRetryAt = DateTime.UtcNow.AddHours(RetryCount switch
        {
            1 => 24,
            2 => 72,
            _ => 168
        });
        UpdatedAt = DateTime.UtcNow;
    }
}

public sealed class SubscriptionInvoice : Entity<Guid>
{
    public Guid SubscriptionId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private SubscriptionInvoice() : base(Guid.NewGuid()) { }
}

public enum BillingInterval { Daily, Weekly, Monthly, Quarterly, Annual }

public enum SubscriptionStatus
{
    Active,
    Trialing,
    PastDue,
    Cancelled,
    Unpaid,
    Paused
}

public enum InvoiceStatus { Draft, Open, Paid, Void, Uncollectible }
