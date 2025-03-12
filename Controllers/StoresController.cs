using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStores(
            [FromQuery] long? StoreId, 
            [FromQuery] string? StoreName, 
            [FromQuery] string? StoreLocation,
            [FromQuery] string? PhoneNumber, 
            [FromQuery] int? Status, 
            [FromQuery] int? AreaId,
            [FromQuery] string sortBy = "StoreId", 
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var (stores, pagination) = await _storeService.GetStoresAsync(StoreId, StoreName, StoreLocation, PhoneNumber, Status, AreaId, sortBy, isAscending, page, pageSize);
            return Ok(new { Stores = stores, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(long id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore(Store store)
        {
            var createdStore = await _storeService.CreateStoreAsync(store);
            return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.StoreId }, createdStore);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(long id, Store store)
        {
            var updatedStore = await _storeService.UpdateStoreAsync(id, store);
            if (updatedStore == null)
            {
                return NotFound();
            }
            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(long id)
        {
            var result = await _storeService.DeleteStoreAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}