namespace ApproveMe.Models.Transactions;

public class TransactionViewModel
{
    public required Guid Id { get; set; }
    public required string Status { get; set; }
    public required string Description { get; set; }
    
    public static TransactionViewModel FromTransaction(TransactionAggregate transactionAggregate)
    {
        var viewModel = new TransactionViewModel
        {
            Id = transactionAggregate.Id,
            Status = transactionAggregate.Status.ToString(),
            Description = transactionAggregate.Content,
        };

        return viewModel;
    }
}