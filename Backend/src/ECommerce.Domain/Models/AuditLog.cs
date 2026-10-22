namespace ECommerce.Domain.Models;

public class AuditLog
{
    private AuditLog()
    {
    }

    private AuditLog(string userEmail, string action, string entity, string? entityId, string? details)
    {
        Id = Guid.NewGuid();
        UserEmail = userEmail;
        Action = action;
        Entity = entity;
        EntityId = entityId;
        Details = details;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string UserEmail { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string Entity { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? Details { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static AuditLog Create(string userEmail, string action, string entity, string? entityId = null, string? details = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty.", nameof(action));

        if (string.IsNullOrWhiteSpace(entity))
            throw new ArgumentException("Entity cannot be null or empty.", nameof(entity));

        return new AuditLog(userEmail, action, entity, entityId, details);
    }
}