using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineProductController : ControllerBase
    {
        private readonly IMachineProductService _machineProductService;

        public MachineProductController(IMachineProductService machineProductService)
        {
            _machineProductService = machineProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineProducts(
            [FromQuery] int? MachineProductId, [FromQuery] int? MachineStockQuantity, [FromQuery] int? Status,
            [FromQuery] int? MachineId, [FromQuery] int? ProductId,
            [FromQuery] string sortBy = "MachineProductId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (machineProducts, pagination) = await _machineProductService.GetMachineProductsAsync(MachineProductId, MachineStockQuantity, Status, MachineId, ProductId, sortBy, isAscending, page, pageSize);
            return Ok(new { MachineProducts = machineProducts, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMachineProductById(int id)
        {
            var machineProduct = await _machineProductService.GetMachineProductByIdAsync(id);
            if (machineProduct == null)
            {
                return NotFound();
            }
            return Ok(machineProduct);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachineProduct(MachineProduct machineProduct)
        {
            var createdMachineProduct = await _machineProductService.CreateMachineProductAsync(machineProduct);
            return CreatedAtAction(nameof(GetMachineProductById), new { id = createdMachineProduct.MachineProductId }, createdMachineProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachineProduct(int id, MachineProduct machineProduct)
        {
            var updatedMachineProduct = await _machineProductService.UpdateMachineProductAsync(id, machineProduct);
            if (updatedMachineProduct == null)
            {
                return NotFound();
            }
            return Ok(updatedMachineProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineProduct(int id)
        {
            var result = await _machineProductService.DeleteMachineProductAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}