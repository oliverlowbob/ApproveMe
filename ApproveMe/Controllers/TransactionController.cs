using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApproveMe.Models.Transactions;
using ApproveMe.Repositories;
using Marten;

namespace ApproveMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController(IRepository repository, IDocumentSession session) : ControllerBase
    {
        [HttpGet]
        public async Task<IResult> GetAll()
        {
            var transactions = await session.Query<TransactionAggregate>().ToListAsync();
            
            var viewModels = transactions.Select(TransactionViewModel.FromTransaction);
            return TypedResults.Ok(viewModels);
        }

        [HttpPost("{id}/approve")]
        public async Task<IResult> ApproveTransaction(Guid id)
        {
            var transaction = await repository.Load<TransactionAggregate>(id);
            
            if (transaction == null)
            {
                return TypedResults.NotFound("Transaction not found.");
            }

            if (transaction.Status == TransactionStatus.Approved)
            {
                return TypedResults.BadRequest("This transaction has already been approved.");
            }

            transaction.Approve(User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown");
            
            await repository.Save(transaction);
            
            return TypedResults.Ok(TransactionViewModel.FromTransaction(transaction));
        }

        [HttpPost("{id}/deny")]
        public async Task<IResult> DenyTransaction(Guid id)
        {
            var transaction = await repository.Load<TransactionAggregate>(id);
            
            if (transaction == null)
            {
                return TypedResults.NotFound("Transaction not found.");
            }

            if (transaction.Status != TransactionStatus.Denied)
            {
                return TypedResults.BadRequest("This transaction has already been denied.");
            }

            transaction.Deny(User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown");
            
            await repository.Save(transaction);
            
            return TypedResults.Ok(TransactionViewModel.FromTransaction(transaction));
        }
    }
}

