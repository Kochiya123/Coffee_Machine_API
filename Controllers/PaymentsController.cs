using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments(
            [FromQuery] int? PaymentId, 
            [FromQuery] string? PaymentMethod, 
            [FromQuery] DateTime? PaymentDate,
            [FromQuery] int? PaymentStatus, 
            [FromQuery] int? Status, 
            [FromQuery] int? OrderId,
            [FromQuery] string sortBy = "PaymentId", 
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var (payments, pagination) = await _paymentService.GetPaymentsAsync(PaymentId, PaymentMethod, PaymentDate, PaymentStatus, Status, OrderId, sortBy, isAscending, page, pageSize);
            return Ok(new { Payments = payments, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(Payment payment)
        {
            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.PaymentId }, createdPayment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, Payment payment)
        {
            var updatedPayment = await _paymentService.UpdatePaymentAsync(id, payment);
            if (updatedPayment == null)
            {
                return NotFound();
            }
            return Ok(updatedPayment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}