using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApproveMe.Models.Transactions;

namespace ApproveMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private static readonly List<Transaction> _transactions = new()
        {
            new() { Id = Guid.NewGuid(), Comment = "Transaction 1" },
            new() { Id = Guid.NewGuid(), Comment = "Transaction 2" },
            new() { Id = Guid.NewGuid(), Comment = "Transaction 3" }
        };

        // GET: api/transaction
        [HttpGet]
        public IActionResult GetAll()
        {
            var viewModels = _transactions.Select(TransactionViewModel.FromTransaction);
            return Ok(viewModels);
        }

        // POST: api/transaction/{id}/approve
        [HttpPost("{id}/approve")]
        public IActionResult ApproveTransaction(Guid id)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            if (transaction.Status == TransactionStatus.Approved)
            {
                return BadRequest("This transaction has already been approved.");
            }

            transaction.Status = TransactionStatus.Approved;
            transaction.ActionedBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
            transaction.ActionedAtUtc = DateTime.UtcNow;

            return Ok(TransactionViewModel.FromTransaction(transaction));
        }

        // POST: api/transaction/{id}/deny
        [HttpPost("{id}/deny")]
        public IActionResult DenyTransaction(Guid id)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            if (transaction.Status != TransactionStatus.Denied)
            {
                return BadRequest("This transaction has already been denied.");
            }

            transaction.Status = TransactionStatus.Denied;
            transaction.ActionedBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
            transaction.ActionedAtUtc = DateTime.UtcNow;

            return Ok(TransactionViewModel.FromTransaction(transaction));
        }
    }
}

