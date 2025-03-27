using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/machine/{machineid}/issue")]
    public class MachineIssueController : ControllerBase
    {
        private readonly IMachineIssueService _machineIssueService;

        public MachineIssueController(IMachineIssueService machineIssueService)
        {
            _machineIssueService = machineIssueService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<MachineIssueDto>, PaginationMetadata)>> GetMachineIssues(
            [FromQuery] int? machineId,
            [FromQuery] long? reportedBy,
            [FromQuery] int? status,
            [FromQuery] DateTime? reportDate,
            [FromQuery] string sortBy = "IssueId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (issues, pagination) = await _machineIssueService.GetMachineIssuesAsync(machineId, reportedBy, status, reportDate, sortBy, isAscending, page, pageSize);
            return Ok(new { Issues = issues, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineIssueDto>> GetMachineIssue(int id)
        {
            var issue = await _machineIssueService.GetMachineIssueByIdAsync(id);
            if (issue == null)
                return NotFound();

            return Ok(issue);
        }

        [HttpPost]
        public async Task<ActionResult<MachineIssueDto>> CreateMachineIssue([FromBody] MachineIssueDto issueDto)
        {
            var createdIssue = await _machineIssueService.CreateMachineIssueAsync(issueDto);
            return CreatedAtAction(nameof(GetMachineIssue), new { id = createdIssue.IssueId }, createdIssue);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MachineIssueDto>> UpdateMachineIssue(int id, [FromBody] MachineIssueDto issueDto)
        {
            if (id != issueDto.IssueId)
                return BadRequest("ID mismatch");

            var updatedIssue = await _machineIssueService.UpdateMachineIssueAsync(id, issueDto);
            if (updatedIssue == null)
                return NotFound();

            return Ok(updatedIssue);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineIssue(int id)
        {
            var result = await _machineIssueService.DeleteMachineIssueAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}