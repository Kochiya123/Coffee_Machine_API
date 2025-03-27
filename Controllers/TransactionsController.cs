using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<TransactionDto>, PaginationMetadata)>> GetTransactions(
            [FromQuery] decimal? transactionAmount,
            [FromQuery] DateTime? transactionDate,
            [FromQuery] int? transactionType,
            [FromQuery] int? status,
            [FromQuery] long? walletId,
            [FromQuery] int? orderId,
            [FromQuery] int? paymentId,
            [FromQuery] string sortBy = "TransactionId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (transactions, pagination) = await _transactionService.GetTransactionsAsync(transactionAmount, transactionDate, transactionType, status, walletId, orderId, paymentId, sortBy, isAscending, page, pageSize);
            return Ok(new { Transactions = transactions, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransactionById(long id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            var createdTransaction = await _transactionService.CreateTransactionAsync(transactionDto);
            return CreatedAtAction(nameof(GetTransactionById), new { id = createdTransaction.TransactionId }, createdTransaction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(long id, [FromBody] TransactionDto transactionDto)
        {
            if (id != transactionDto.TransactionId)
                return BadRequest("ID mismatch");

            var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionDto);
            if (updatedTransaction == null)
                return NotFound();

            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(long id)
        {
            var result = await _transactionService.DeleteTransactionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}