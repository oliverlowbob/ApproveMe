using ApproveMe.InternalEvents;

namespace ApproveMe.Models.Transactions;

public class TransactionAggregate : AggregateBase
{
    public TransactionStatus Status { get; set; }
    public string Content { get; set; }
    public Guid DataBatchId { get; set; }
    public string? ActionedBy { get; set; }
    public DateTime? ActionedAt { get; set; }
    
    public TransactionAggregate(string content, Guid dataBatchId)                                                               
    {                                                                                                                  
        var @event = new TransactionCreatedEvent(Guid.NewGuid(), DateTime.UtcNow, content, dataBatchId);               
                                                                                                                       
        RaiseEvent(@event);                                                                                   
    }
    
    public void Apply(TransactionCreatedEvent @event)                                                                 
    {                                                                                                                  
        Id = @event.Id;                                                                                              
        Status = @event.Status;                                                                                     
        Content = @event.Content;                                                                                   
        DataBatchId = @event.DataBatchId;                                                                          
    }

    public TransactionAggregate()
    {
    }
    
    public void Approve(string actionedBy)
    {
        if (Status == TransactionStatus.Approved)
        {
            return;
        }
        
        RaiseEvent(new TransactionApprovedEvent(Id, DateTime.UtcNow, actionedBy));
    }
    
    public void Apply(TransactionApprovedEvent @event)
    {
        Status = TransactionStatus.Approved;
        ActionedBy = @event.ActionedBy;
        ActionedAt = @event.Timestamp;
    }

    public void Deny(string actionedBy)
    {                    
        if (Status == TransactionStatus.Denied)
        {                                      
            return;                            
        }                                      
        
        RaiseEvent(new TransactionDeniedEvent(Id, DateTime.UtcNow, actionedBy));  
    }
        
    public void Apply(TransactionDeniedEvent @event)
    {
        Status = TransactionStatus.Denied;
    }
}