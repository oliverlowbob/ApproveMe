namespace ApproveMe.InternalEvents;

public abstract class BaseEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; }

    public BaseEvent(Guid id, DateTime? timestamp)
    {
        Id = id;
        Timestamp = timestamp ?? DateTime.UtcNow;
    }
}
