using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<StoreDto>, PaginationMetadata)>> GetStores(
            [FromQuery] string? storeName,
            [FromQuery] string? storeLocation,
            [FromQuery] string? phoneNumber,
            [FromQuery] int? status,
            [FromQuery] int? areaId,
            [FromQuery] string sortBy = "StoreId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (stores, pagination) = await _storeService.GetStoresAsync(storeName, storeLocation, phoneNumber, status, areaId, sortBy, isAscending, page, pageSize);
            return Ok(new { Stores = stores, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoreDto>> GetStoreById(long id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
                return NotFound();

            return Ok(store);
        }

        [HttpPost]
        public async Task<ActionResult<StoreDto>> CreateStore([FromBody] StoreDto storeDto)
        {
            var createdStore = await _storeService.CreateStoreAsync(storeDto);
            return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.StoreId }, createdStore);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StoreDto>> UpdateStore(long id, [FromBody] StoreDto storeDto)
        {
            if (id != storeDto.StoreId)
                return BadRequest("ID mismatch");

            var updatedStore = await _storeService.UpdateStoreAsync(id, storeDto);
            if (updatedStore == null)
                return NotFound();

            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(long id)
        {
            var result = await _storeService.DeleteStoreAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}