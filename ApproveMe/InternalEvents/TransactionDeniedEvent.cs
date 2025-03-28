namespace ApproveMe.InternalEvents;

public class TransactionDeniedEvent : BaseEvent
{
    public TransactionDeniedEvent(Guid id, DateTime? timestamp, string actionedBy) : base(id, timestamp)
    {
        ActionedBy = actionedBy;
    }

    public string ActionedBy { get; set; }
}