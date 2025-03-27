using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/machine_types")]
    public class MachineTypeController : ControllerBase
    {
        private readonly IMachineTypeService _machineTypeService;

        public MachineTypeController(IMachineTypeService machineTypeService)
        {
            _machineTypeService = machineTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<MachineTypeDto>, PaginationMetadata)>> GetMachineTypes(
            [FromQuery] string? typeName,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "MachineTypeId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (types, pagination) = await _machineTypeService.GetMachineTypesAsync(typeName, status, sortBy, isAscending, page, pageSize);
            return Ok(new { MachineTypes = types, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineTypeDto>> GetMachineType(int id)
        {
            var type = await _machineTypeService.GetMachineTypeByIdAsync(id);
            if (type == null)
                return NotFound();

            return Ok(type);
        }

        [HttpPost]
        public async Task<ActionResult<MachineTypeDto>> CreateMachineType([FromBody] MachineTypeDto typeDto)
        {
            var createdType = await _machineTypeService.CreateMachineTypeAsync(typeDto);
            return CreatedAtAction(nameof(GetMachineType), new { id = createdType.MachineTypeId }, createdType);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MachineTypeDto>> UpdateMachineType(int id, [FromBody] MachineTypeDto typeDto)
        {
            if (id != typeDto.MachineTypeId)
                return BadRequest("ID mismatch");

            var updatedType = await _machineTypeService.UpdateMachineTypeAsync(id, typeDto);
            if (updatedType == null)
                return NotFound();

            return Ok(updatedType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineType(int id)
        {
            var result = await _machineTypeService.DeleteMachineTypeAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}