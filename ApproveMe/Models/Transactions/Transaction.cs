using System.ComponentModel.DataAnnotations;

namespace ApproveMe.Models.Transactions;

public class Transaction
{
    [Key]
    public Guid Id { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public required string Comment { get; set; }
    public string? ActionedBy { get; set; }
    public DateTime? ActionedAtUtc { get; set; }
}