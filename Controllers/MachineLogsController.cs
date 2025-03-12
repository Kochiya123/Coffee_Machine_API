using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineLogController : ControllerBase
    {
        private readonly IMachineLogService _machineLogService;

        public MachineLogController(IMachineLogService machineLogService)
        {
            _machineLogService = machineLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineLogs(
            [FromQuery] int? LogId, [FromQuery] DateTime? LogDate, [FromQuery] string? LogDescription,
            [FromQuery] int? LogType, [FromQuery] int? Status, [FromQuery] int? MachineId, [FromQuery] int? TechnicianId,
            [FromQuery] string sortBy = "LogId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (machineLogs, pagination) = await _machineLogService.GetMachineLogsAsync(LogId, LogDate, LogDescription, LogType, Status, MachineId, TechnicianId, sortBy, isAscending, page, pageSize);
            return Ok(new { MachineLogs = machineLogs, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMachineLogById(int id)
        {
            var machineLog = await _machineLogService.GetMachineLogByIdAsync(id);
            if (machineLog == null)
            {
                return NotFound();
            }
            return Ok(machineLog);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachineLog(MachineLog machineLog)
        {
            var createdMachineLog = await _machineLogService.CreateMachineLogAsync(machineLog);
            return CreatedAtAction(nameof(GetMachineLogById), new { id = createdMachineLog.LogId }, createdMachineLog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachineLog(int id, MachineLog machineLog)
        {
            var updatedMachineLog = await _machineLogService.UpdateMachineLogAsync(id, machineLog);
            if (updatedMachineLog == null)
            {
                return NotFound();
            }
            return Ok(updatedMachineLog);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineLog(int id)
        {
            var result = await _machineLogService.DeleteMachineLogAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}