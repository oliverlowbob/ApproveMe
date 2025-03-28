using ApproveMe.Models.Transactions;

namespace ApproveMe.InternalEvents;

public class TransactionCreatedEvent : BaseEvent
{
    public TransactionCreatedEvent(Guid id, DateTime? timestamp, string content, Guid dataBatchId) : base(id, timestamp)
    {
        Content = content;
        Status = TransactionStatus.Pending;
        DataBatchId = dataBatchId;
    }
    
    public TransactionStatus Status;
    public string Content;
    public Guid DataBatchId;
}