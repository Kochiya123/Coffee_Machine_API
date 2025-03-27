using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;
using WebApplication2.Payments;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly VNPayHelper _vnPayHelper;

        public WalletController(IWalletService walletService, ITransactionService transactionService, IConfiguration configuration, VNPayHelper vnPayHelper)
        {
            _walletService = walletService;
            _transactionService = transactionService;
            _configuration = configuration;
            _vnPayHelper = vnPayHelper;
        }


        [HttpGet("/customer/{customerId}")]
        public async Task<IActionResult> GetWalletByCustomerId(long customerId)
        {
            var walletDto = await _walletService.GetWalletByCustomerIdAsync(customerId);

            if (walletDto == null)
            {
                return NotFound(new { message = "Wallet not found for the given customer ID." });
            }

            return Ok(walletDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WalletDto>> GetWalletById(long id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);
            if (wallet == null)
                return NotFound();
            return Ok(wallet);
        }

        [HttpPost("{walletId}/recharge")]
        public async Task<IActionResult> VNPayWalletRechargeCallback([FromQuery] Dictionary<string, string> queryParams)
        {
            string vnpResponseCode = queryParams["vnp_ResponseCode"]; // Payment status
            string vnpTransactionNo = queryParams["vnp_TransactionNo"]; // VNPay Transaction ID
            string walletId = queryParams["vnp_TxnRef"]; // Wallet ID
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
                var wallet = await _walletService.GetWalletByIdAsync(long.Parse(walletId));
                if (wallet == null)
                {
                    return NotFound("Wallet not found");
                }

                // ✅ Step 1: Add funds to wallet
                wallet.Balance += amount;
                await _walletService.UpdateWalletAsync(wallet.WalletId, wallet);

                // ✅ Step 2: Record transaction
                var transactionDto = new TransactionDto
                {
                    TransactionAmount = amount,
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = 2, // Assuming 2 = Wallet Recharge via VNPay
                    Status = 1, // Active
                    WalletId = wallet.WalletId
                };

                await _transactionService.CreateTransactionAsync(transactionDto);

                return Ok(new { Message = "Wallet recharge successful", WalletBalance = wallet.Balance });
            }
            else
            {
                return BadRequest($"VNPay payment failed with response code: {vnpResponseCode}");
            }
        }

        [HttpPost("{walletId}/vnpay")]
        public IActionResult CreateVNPayWalletRecharge(long walletId, [FromBody] WalletRechargeRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Invalid recharge amount");
            }

            // Generate VNPay payment URL
            string paymentUrl = _vnPayHelper.CreatePaymentUrl(HttpContext, request.Amount, walletId.ToString());

            return Ok(new { PaymentUrl = paymentUrl });
        }


        // ✅ Wallet Recharge Request DTO
        public class WalletRechargeRequest
        {
            public decimal Amount { get; set; }
        }


        [HttpPost]
        public async Task<ActionResult<WalletDto>> CreateWallet([FromBody] WalletDto walletDto)
        {
            var createdWallet = await _walletService.CreateWalletAsync(walletDto);
            return CreatedAtAction(nameof(GetWalletById), new { id = createdWallet.WalletId }, createdWallet);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WalletDto>> UpdateWallet(long id, [FromBody] WalletDto walletDto)
        {
            if (id != walletDto.WalletId)
                return BadRequest("ID mismatch");

            var updatedWallet = await _walletService.UpdateWalletAsync(id, walletDto);
            if (updatedWallet == null)
                return NotFound();

            return Ok(updatedWallet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(long id)
        {
            var result = await _walletService.DeleteWalletAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}