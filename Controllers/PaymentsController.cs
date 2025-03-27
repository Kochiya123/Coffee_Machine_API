using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;
using WebApplication2.Payments;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderRepository;
        private readonly VNPayHelper _vnPayHelper;
        private readonly IWalletService _walletService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService, IConfiguration configuration, IWalletService walletService,VNPayHelper vNPayHelper)
        {
            _paymentService = paymentService;
            _orderRepository = orderService;
            _configuration = configuration;
            _walletService = walletService;
            _vnPayHelper = vNPayHelper;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<PaymentDto>, PaginationMetadata)>> GetPayments(
            [FromQuery] string? paymentCode,
            [FromQuery] string? paymentMethod,
            [FromQuery] DateTime? paymentDate,
            [FromQuery] int? paymentStatus,
            [FromQuery] int? status,
            [FromQuery] int? orderId,
            [FromQuery] string sortBy = "PaymentId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (payments, pagination) = await _paymentService.GetPaymentsAsync(paymentCode, paymentMethod, paymentDate, paymentStatus, status, orderId, sortBy, isAscending, page, pageSize);
            return Ok(new { Payments = payments, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }



        [HttpPost("vnpay")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            // Retrieve the order from the database
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            
            if(order.Status == 1)
            {
                return BadRequest("Order already confirmed!");
            }

            // Ensure the amount matches the order total
            if (request.Amount != order.TotalAmount)
            {
                return BadRequest("Invalid payment amount");
            }

            // Generate VNPay payment URL
            string paymentUrl = _vnPayHelper.CreatePaymentUrl(HttpContext, request.Amount, request.OrderId.ToString()) ;

            if (paymentUrl == null)
            {
                return BadRequest("Can't generate URL");
            }

            return Ok(new { PaymentUrl = paymentUrl });
        }

        // Request DTO
        public class PaymentRequest
        {
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
        }

        [HttpGet("/vnpay")]
        public async Task<IActionResult> VNPayReturn([FromQuery] Dictionary<string, string> queryParams)
        {
            string vnpResponseCode = queryParams["vnp_ResponseCode"]; // Payment status
            string vnpTransactionNo = queryParams["vnp_TransactionNo"]; // VNPay Transaction ID
            string orderId = queryParams["vnp_TxnRef"]; // Order ID
            decimal amount = decimal.Parse(queryParams["vnp_Amount"]) / 100; // Convert from VND * 100
            string vnpSecureHash = queryParams["vnp_SecureHash"];
            queryParams.Remove("vnp_SecureHash");

            // Validate signature
            string secretKey = _configuration["VNPaySettings:HashSecret"];
            string signData = string.Join("&", queryParams.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}={kvp.Value}"));
            string expectedHash = VNPayHelper.ComputeHmacSha512(secretKey, signData);

            if (expectedHash != vnpSecureHash)
            {
                return BadRequest("Invalid signature");
            }

            if (vnpResponseCode == "00") // Payment successful
            {
                var paymentDto = new PaymentDto
                {
                    PaymentCode = vnpTransactionNo,
                    PaymentMethod = "VNPay",
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = 1, // Success
                    Status = 1, // Active
                    OrderId = int.Parse(orderId),
                };

                var savedPayment = await _paymentService.CreatePaymentAsync(paymentDto);

                // Mark the order as paid
                var orderDto = await _orderRepository.GetOrderByIdAsync(paymentDto.OrderId);
                if (orderDto != null)
                {
                    orderDto.Status = 2; // Assuming 2 = Paid
                    await _orderRepository.UpdateOrderAsync(paymentDto.OrderId,orderDto);
                }

                return Ok(new { Message = "Payment successful", Payment = savedPayment });
            }
            else
            {
                return BadRequest($"Payment failed with response code: {vnpResponseCode}");
            }
        }

        [HttpPost("wallet")]
        public async Task<IActionResult> CreateWalletPayment([FromBody] WalletPaymentRequest request)
        {
            // ✅ Step 1: Retrieve the order from the database
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // ✅ Step 2: Get the customer's wallet
            var wallet = await _walletService.GetWalletByCustomerIdAsync(order.CustomerId);
            if (wallet == null)
            {
                return NotFound("Customer wallet not found");
            }

            // ✅ Step 3: Check if the wallet has enough balance
            if (wallet.Balance < order.TotalAmount)
            {
                return BadRequest("Insufficient wallet balance");
            }

            // ✅ Step 4: Deduct the amount from the wallet
            wallet.Balance -= order.TotalAmount;
            await _walletService.UpdateWalletAsync(wallet.WalletId, wallet);

            // ✅ Step 5: Create the payment record
            var paymentDto = new PaymentDto
            {
                PaymentCode = $"WALLET-{Guid.NewGuid()}",
                PaymentMethod = "Wallet",
                PaymentDate = DateTime.UtcNow,
                PaymentStatus = 1, // Success
                Status = 1, // Active
                OrderId = order.OrderId
            };

            var savedPayment = await _paymentService.CreatePaymentAsync(paymentDto);

            // ✅ Step 6: Update order status to Paid
            var updatedOrderDto = new OrderDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                Status = 2 // Assuming 2 = Paid
            };

            await _orderRepository.UpdateOrderAsync(order.OrderId, updatedOrderDto);

            return Ok(new { Message = "Payment successful", Payment = savedPayment, WalletBalance = wallet.Balance });
        }


        public class WalletPaymentRequest
        {
            public int OrderId { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDto>> UpdatePayment(int id, [FromBody] PaymentDto paymentDto)
        {
            if (id != paymentDto.PaymentId)
                return BadRequest("ID mismatch");

            var updatedPayment = await _paymentService.UpdatePaymentAsync(id, paymentDto);
            if (updatedPayment == null)
                return NotFound();

            return Ok(updatedPayment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}