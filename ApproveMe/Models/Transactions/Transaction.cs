using System.ComponentModel.DataAnnotations;

namespace ApproveMe.Models.Transactions;

public class Transaction
{
    [Key]
    public Guid Id { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    [Required]
    public required string Content { get; set; }
    public string? ActionedBy { get; set; }
    public DateTime? ActionedAtUtc { get; set; }
    public Guid DataBatchId { get; set; }
    public DataBatch.DataBatch DataBatch { get; set; } = null!;
}