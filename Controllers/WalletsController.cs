using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // GET: api/Wallet
        [HttpGet]
        public async Task<IActionResult> GetWallets(
            [FromQuery] long? customerId, 
            [FromQuery] int? status,
            [FromQuery] string sortBy = "WalletId", 
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var (wallets, pagination) = await _walletService.GetWalletsAsync(customerId, status, sortBy, isAscending, page, pageSize);
            return Ok(new { Wallets = wallets, Pagination = pagination });
        }

        // GET: api/Wallet/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(long id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        // POST: api/Wallet
        [HttpPost]
        public async Task<IActionResult> CreateWallet(WalletDto walletDto)
        {
            var createdWallet = await _walletService.CreateWalletAsync(walletDto);
            return CreatedAtAction(nameof(GetWalletById), new { id = createdWallet.WalletId }, createdWallet);
        }

        // PUT: api/Wallet/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(long id, WalletDto walletDto)
        {
            if (id != walletDto.WalletId)
            {
                return BadRequest("Wallet ID mismatch.");
            }

            var updatedWallet = await _walletService.UpdateWalletAsync(id, walletDto);
            if (updatedWallet == null)
            {
                return NotFound();
            }
            return Ok(updatedWallet);
        }

        // DELETE: api/Wallet/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(long id)
        {
            var result = await _walletService.DeleteWalletAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}