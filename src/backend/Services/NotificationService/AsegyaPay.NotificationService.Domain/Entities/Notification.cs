using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.NotificationService.Domain.Entities;

/// <summary>
/// Notification entity — tracks outbound notification delivery.
/// </summary>
public sealed class Notification : Entity<Guid>
{
    public string RecipientId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public string? RecipientAddress { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime? NextAttemptAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    private Notification() : base(Guid.NewGuid()) { }

    public static Notification Create(
        string recipientId,
        NotificationChannel channel,
        string subject,
        string body,
        string? recipientAddress = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = recipientId,
            Channel = channel,
            Subject = subject,
            Body = body,
            RecipientAddress = recipientAddress,
            Status = NotificationStatus.Pending,
            AttemptCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkDelivered()
    {
        Status = NotificationStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string error, DateTime? retryAt = null)
    {
        AttemptCount++;
        ErrorMessage = error;
        Status = retryAt.HasValue ? NotificationStatus.Retrying : NotificationStatus.Failed;
        NextAttemptAt = retryAt;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum NotificationChannel { Email, Sms, Webhook, Push }

public enum NotificationStatus { Pending, Delivered, Retrying, Failed }
