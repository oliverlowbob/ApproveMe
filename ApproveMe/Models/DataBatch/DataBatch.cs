using System.ComponentModel.DataAnnotations;
using ApproveMe.Models.Transactions;

namespace ApproveMe.Models.DataBatch;

public class DataBatch
{
    public Guid Id { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public string CreatedBy { get; set; }
}