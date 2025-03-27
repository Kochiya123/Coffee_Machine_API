using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/machine/{machineId}/product")]
    public class MachineProductController : ControllerBase
    {
        private readonly IMachineProductService _machineProductService;

        public MachineProductController(IMachineProductService machineProductService)
        {
            _machineProductService = machineProductService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<MachineProductDto>, PaginationMetadata)>> GetMachineProducts(
            [FromQuery] int? machineId,
            [FromQuery] int? productId,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "MachineProductId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (products, pagination) = await _machineProductService.GetMachineProductsAsync(machineId, productId, status, sortBy, isAscending, page, pageSize);
            return Ok(new { MachineProducts = products, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineProductDto>> GetMachineProduct(int id)
        {
            var product = await _machineProductService.GetMachineProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<MachineProductDto>> CreateMachineProduct([FromBody] MachineProductDto productDto)
        {
            var createdProduct = await _machineProductService.CreateMachineProductAsync(productDto);
            return CreatedAtAction(nameof(GetMachineProduct), new { id = createdProduct.MachineProductId }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MachineProductDto>> UpdateMachineProduct(int id, [FromBody] MachineProductDto productDto)
        {
            if (id != productDto.MachineProductId)
                return BadRequest("ID mismatch");

            var updatedProduct = await _machineProductService.UpdateMachineProductAsync(id, productDto);
            if (updatedProduct == null)
                return NotFound();

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineProduct(int id)
        {
            var result = await _machineProductService.DeleteMachineProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}