using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/machine")]
    public class MachineController : ControllerBase
    {
        private readonly IMachineService _machineService;

        public MachineController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMachines(
            [FromQuery] int? MachineId, 
            [FromQuery] string? MachineName, 
            [FromQuery] DateOnly? InstallationDate,
            [FromQuery] int? Status, 
            [FromQuery] long? StoreId, 
            [FromQuery] int? MachineTypeId,
            [FromQuery] string sortBy = "MachineId", 
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var (machines, pagination) = await _machineService.GetMachinesAsync(MachineId, MachineName, InstallationDate, Status, StoreId, MachineTypeId, sortBy, isAscending, page, pageSize);
            return Ok(new { Machines = machines, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMachineById(int id)
        {
            var machine = await _machineService.GetMachineByIdAsync(id);
            if (machine == null)
            {
                return NotFound();
            }
            return Ok(machine);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachine(Machine machine)
        {
            var createdMachine = await _machineService.CreateMachineAsync(machine);
            return CreatedAtAction(nameof(GetMachineById), new { id = createdMachine.MachineId }, createdMachine);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachine(int id, Machine machine)
        {
            var updatedMachine = await _machineService.UpdateMachineAsync(id, machine);
            if (updatedMachine == null)
            {
                return NotFound();
            }
            return Ok(updatedMachine);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var result = await _machineService.DeleteMachineAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}