namespace EventManager.Domain.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public string EntityName { get; }
    public Guid EventId { get; }

    public EntityNotFoundException(string entityName) : base($"{entityName} is not found")
    {
        EntityName = entityName;
    }

    public EntityNotFoundException(string entityName, Guid entityId) : base($"{entityName} {entityId} is not found")
    {
        EntityName = entityName;
        EventId = entityId;
    }

    public EntityNotFoundException(string entityName, Guid eventId, Exception inner) 
        : base($"{entityName} {eventId} is not found", inner)
    {
        EntityName = entityName;
        EventId = eventId;
    }
}
