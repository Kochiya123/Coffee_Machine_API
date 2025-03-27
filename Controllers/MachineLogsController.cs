using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/machine{machineId}/log")]
    public class MachineLogController : ControllerBase
    {
        private readonly IMachineLogService _machineLogService;

        public MachineLogController(IMachineLogService machineLogService)
        {
            _machineLogService = machineLogService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<MachineLogDto>, PaginationMetadata)>> GetMachineLogs(
            [FromQuery] int? machineId,
            [FromQuery] int? technicianId,
            [FromQuery] long? performedBy,
            [FromQuery] int? logType,
            [FromQuery] int? status,
            [FromQuery] DateTime? logDate,
            [FromQuery] string sortBy = "LogId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (logs, pagination) = await _machineLogService.GetMachineLogsAsync(machineId, technicianId, performedBy, logType, status, logDate, sortBy, isAscending, page, pageSize);
            return Ok(new { Logs = logs, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineLogDto>> GetMachineLog(int id)
        {
            var log = await _machineLogService.GetMachineLogByIdAsync(id);
            if (log == null)
                return NotFound();

            return Ok(log);
        }

        [HttpPost]
        public async Task<ActionResult<MachineLogDto>> CreateMachineLog([FromBody] MachineLogDto logDto)
        {
            var createdLog = await _machineLogService.CreateMachineLogAsync(logDto);
            return CreatedAtAction(nameof(GetMachineLog), new { id = createdLog.LogId }, createdLog);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MachineLogDto>> UpdateMachineLog(int id, [FromBody] MachineLogDto logDto)
        {
            if (id != logDto.LogId)
                return BadRequest("ID mismatch");

            var updatedLog = await _machineLogService.UpdateMachineLogAsync(id, logDto);
            if (updatedLog == null)
                return NotFound();

            return Ok(updatedLog);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineLog(int id)
        {
            var result = await _machineLogService.DeleteMachineLogAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}