namespace RealEstateApi.Domain.Entities;

public class AuditLog
{
    public int Id { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty; // Create, Update, Delete
    public string UserId { get; private set; } = string.Empty; // Who did it
    public string Details { get; private set; } = string.Empty; // JSON
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(string entityName, string entityId, string action, string userId, string details)
    {
        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }
}
