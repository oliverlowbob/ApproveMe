namespace ApproveMe.InternalEvents;

public abstract class BaseEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }

    protected BaseEvent(Guid id, DateTime? timestamp = null)
    {
        Id = id;
        Timestamp = timestamp ?? DateTime.UtcNow;
    }
}
