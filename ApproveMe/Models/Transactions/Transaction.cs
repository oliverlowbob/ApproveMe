using ApproveMe.InternalEvents;

namespace ApproveMe.Models.Transactions;

public class Transaction : AggregateBase
{
    public TransactionStatus Status { get; set; }
    public string Content { get; set; }
    public Guid DataBatchId { get; set; }
    public string? ActionedBy { get; set; }
    public DateTime? ActionedAt { get; set; }
    
    public Transaction(string content, Guid dataBatchId)                                                               
    {                                                                                                                  
        var @event = new TransactionCreatedEvent(Guid.NewGuid(), DateTime.UtcNow, content, dataBatchId);               
                                                                                                                       
        Apply(@event);                                                                                                 
                                                                                                                       
        AddUncommittedEvent(@event);                                                                                   
    }        
    
    private void Apply(TransactionCreatedEvent @event)      
    {                                                       
        Id = @event.Id;                                     
        Content = @event.Content;                           
        DataBatchId = @event.DataBatchId;                   
        Status = @event.Status;                             
                                                            
        Version++;                                          
    }                                                       
    
    public void Approve(string actionedBy)
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new InvalidOperationException("Transaction is not pending");
        }
        
        AddUncommittedEvent(new TransactionApprovedEvent(Id, DateTime.UtcNow, actionedBy));
    }
    
    private void Apply(TransactionApprovedEvent @event)
    {
        Status = TransactionStatus.Approved;
        ActionedBy = @event.ActionedBy;
        ActionedAt = @event.Timestamp;
        
        Version++;
    }

    public void Deny(string actionedBy)
    {
        if (Status != TransactionStatus.Pending)                                             
        {                                                                                    
            throw new InvalidOperationException("Transaction is not pending");               
        }                                                                                    
                                                                                             
        AddUncommittedEvent(new TransactionDeniedEvent(Id, DateTime.UtcNow, actionedBy));  
    }
    
    private void Apply(TransactionDeniedEvent @event)
    {
        Status = TransactionStatus.Denied;
        ActionedBy = @event.ActionedBy;
        ActionedAt = @event.Timestamp;
        
        Version++;
    }
}