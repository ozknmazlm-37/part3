namespace App.Core;

public enum Role
{
    Admin,
    Office,
    Field
}

public enum CofrePasswordStatus
{
    Active,
    Deprecated,
    NeedsConfirm
}

public sealed class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    public long? TelegramUserId { get; set; }
    public long? TelegramChatId { get; set; }
    public string? WhatsAppId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset? LastLoginAt { get; set; }
}

public sealed class Cofre
{
    public int CofreNo { get; set; }
    public string? BuildingName { get; set; }
    public string? Address { get; set; }
    public double? XCoord { get; set; }
    public double? YCoord { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Meter
{
    public string MeterSerialNo { get; set; } = string.Empty;
    public string? SubscriberNo { get; set; }
    public string? SubscriberName { get; set; }
    public int CofreNo { get; set; }
    public double? XCoord { get; set; }
    public double? YCoord { get; set; }
    public Guid? ImportBatchId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class CofrePassword
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int CofreNo { get; set; }
    public string PasswordValue { get; set; } = string.Empty;
    public string? Note { get; set; }
    public CofrePasswordStatus Status { get; set; } = CofrePasswordStatus.Active;
    public Guid CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid UpdatedByUserId { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? ConfirmedByUserId { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
}

public sealed class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? OldValueJson { get; set; }
    public string? NewValueJson { get; set; }
    public Guid? ActorUserId { get; set; }
    public string ActorNameSnapshot { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string? Extra { get; set; }
}

public sealed class AppSetting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
