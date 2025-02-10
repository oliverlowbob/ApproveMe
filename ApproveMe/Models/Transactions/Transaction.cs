namespace ApproveMe.Models.Transactions;

public class Transaction
{
    public int Id { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public required string Description { get; set; }
    public string? ActionedBy { get; set; }
    public DateTime? ActionedAtUtc { get; set; }
}