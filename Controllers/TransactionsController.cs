using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] long? TransactionId, [FromQuery] decimal? TransactionAmount, [FromQuery] DateTime? TransactionDate,
            [FromQuery] int? TransactionType, [FromQuery] int? Status, [FromQuery] long? WalletId, [FromQuery] int? OrderId, [FromQuery] int? PaymentId,
            [FromQuery] string sortBy = "TransactionId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (transactions, pagination) = await _transactionService.GetTransactionsAsync(TransactionId, TransactionAmount, TransactionDate, TransactionType, Status, WalletId, OrderId, PaymentId, sortBy, isAscending, page, pageSize);
            return Ok(new { Transactions = transactions, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(long id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(Transaction transaction)
        {
            var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetTransactionById), new { id = createdTransaction.TransactionId }, createdTransaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(long id, Transaction transaction)
        {
            var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transaction);
            if (updatedTransaction == null)
            {
                return NotFound();
            }
            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(long id)
        {
            var result = await _transactionService.DeleteTransactionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}