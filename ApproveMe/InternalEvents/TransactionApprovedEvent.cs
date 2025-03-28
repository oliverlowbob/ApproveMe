namespace ApproveMe.InternalEvents;

public class TransactionApprovedEvent : BaseEvent
{
    public TransactionApprovedEvent(Guid id, DateTime timestamp, string actionedBy) : base(id, timestamp)
    {
        ActionedBy = actionedBy;
    }
    
    public string ActionedBy { get; set; }
}