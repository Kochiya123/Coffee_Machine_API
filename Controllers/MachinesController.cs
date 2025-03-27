using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/machines")]
    public class MachineController : ControllerBase
    {
        private readonly IMachineService _machineService;

        public MachineController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<MachineDto>, PaginationMetadata)>> GetMachines(
            [FromQuery] string? machineCode,
            [FromQuery] string? machineName,
            [FromQuery] DateOnly? installationDate,
            [FromQuery] int? status,
            [FromQuery] long? storeId,
            [FromQuery] int? machineTypeId,
            [FromQuery] string sortBy = "MachineId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (machines, pagination) = await _machineService.GetMachinesAsync(machineCode, machineName, installationDate, status, storeId, machineTypeId, sortBy, isAscending, page, pageSize);
            return Ok(new { Machines = machines, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineDto>> GetMachine(int id)
        {
            var machine = await _machineService.GetMachineByIdAsync(id);
            if (machine == null)
                return NotFound();

            return Ok(machine);
        }

        [HttpPost]
        public async Task<ActionResult<MachineDto>> CreateMachine([FromBody] MachineDto machineDto)
        {
            var createdMachine = await _machineService.CreateMachineAsync(machineDto);
            return CreatedAtAction(nameof(GetMachine), new { id = createdMachine.MachineId }, createdMachine);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MachineDto>> UpdateMachine(int id, [FromBody] MachineDto machineDto)
        {
            if (id != machineDto.MachineId)
                return BadRequest("ID mismatch");

            var updatedMachine = await _machineService.UpdateMachineAsync(id, machineDto);
            if (updatedMachine == null)
                return NotFound();

            return Ok(updatedMachine);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var result = await _machineService.DeleteMachineAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}