namespace ApproveMe.Models.Transactions;

public class TransactionViewModel
{
    public required Guid Id { get; set; }
    public required string Status { get; set; }
    public required string Description { get; set; }
    
    public static TransactionViewModel FromTransaction(Transaction transaction)
    {
        var viewModel = new TransactionViewModel
        {
            Id = transaction.Id,
            Status = transaction.Status.ToString(),
            Description = transaction.Comment,
        };

        return viewModel;
    }
}