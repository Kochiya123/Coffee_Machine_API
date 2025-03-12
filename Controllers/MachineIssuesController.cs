using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineIssueController : ControllerBase
    {
        private readonly IMachineIssueService _machineIssueService;

        public MachineIssueController(IMachineIssueService machineIssueService)
        {
            _machineIssueService = machineIssueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineIssues(
            [FromQuery] int? IssueId, [FromQuery] DateTime? ReportDate, [FromQuery] string? IssueDescription,
            [FromQuery] int? Status, [FromQuery] int? MachineId, [FromQuery] long? ReportedBy,
            [FromQuery] string sortBy = "IssueId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (machineIssues, pagination) = await _machineIssueService.GetMachineIssuesAsync(IssueId, ReportDate, IssueDescription, Status, MachineId, ReportedBy, sortBy, isAscending, page, pageSize);
            return Ok(new { MachineIssues = machineIssues, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMachineIssueById(int id)
        {
            var machineIssue = await _machineIssueService.GetMachineIssueByIdAsync(id);
            if (machineIssue == null)
            {
                return NotFound();
            }
            return Ok(machineIssue);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachineIssue(MachineIssue machineIssue)
        {
            var createdMachineIssue = await _machineIssueService.CreateMachineIssueAsync(machineIssue);
            return CreatedAtAction(nameof(GetMachineIssueById), new { id = createdMachineIssue.IssueId }, createdMachineIssue);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachineIssue(int id, MachineIssue machineIssue)
        {
            var updatedMachineIssue = await _machineIssueService.UpdateMachineIssueAsync(id, machineIssue);
            if (updatedMachineIssue == null)
            {
                return NotFound();
            }
            return Ok(updatedMachineIssue);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineIssue(int id)
        {
            var result = await _machineIssueService.DeleteMachineIssueAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}