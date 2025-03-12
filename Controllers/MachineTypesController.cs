using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineTypeController : ControllerBase
    {
        private readonly IMachineTypeService _machineTypeService;

        public MachineTypeController(IMachineTypeService machineTypeService)
        {
            _machineTypeService = machineTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineTypes(
            [FromQuery] int? MachineTypeId, [FromQuery] string? TypeName, [FromQuery] int? Status,
            [FromQuery] string sortBy = "MachineTypeId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (machineTypes, pagination) = await _machineTypeService.GetMachineTypesAsync(MachineTypeId, TypeName, Status, sortBy, isAscending, page, pageSize);
            return Ok(new { MachineTypes = machineTypes, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMachineTypeById(int id)
        {
            var machineType = await _machineTypeService.GetMachineTypeByIdAsync(id);
            if (machineType == null)
            {
                return NotFound();
            }
            return Ok(machineType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachineType(MachineType machineType)
        {
            var createdMachineType = await _machineTypeService.CreateMachineTypeAsync(machineType);
            return CreatedAtAction(nameof(GetMachineTypeById), new { id = createdMachineType.MachineTypeId }, createdMachineType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachineType(int id, MachineType machineType)
        {
            var updatedMachineType = await _machineTypeService.UpdateMachineTypeAsync(id, machineType);
            if (updatedMachineType == null)
            {
                return NotFound();
            }
            return Ok(updatedMachineType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineType(int id)
        {
            var result = await _machineTypeService.DeleteMachineTypeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}