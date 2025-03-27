using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/issue/{issueId}/assignment")]


    public class IssueAssignmentController : ControllerBase
    {
        private readonly IIssueAssignmentService _issueAssignmentService;

        public IssueAssignmentController(IIssueAssignmentService issueAssignmentService)
        {
            _issueAssignmentService = issueAssignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<IssueAssignmentDto>, PaginationMetadata)>> GetIssueAssignments(
            [FromQuery] int? issueId,
            [FromQuery] int? technicianId,
            [FromQuery] int? status,
            [FromQuery] DateTime? assignedDate,
            [FromQuery] string sortBy = "AssignmentId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (assignments, pagination) = await _issueAssignmentService.GetIssueAssignmentsAsync(issueId, technicianId, status, assignedDate, sortBy, isAscending, page, pageSize);
            return Ok(new { Assignments = assignments, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IssueAssignmentDto>> GetIssueAssignment(int id)
        {
            var assignment = await _issueAssignmentService.GetIssueAssignmentByIdAsync(id);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpPost]
        public async Task<ActionResult<IssueAssignmentDto>> CreateIssueAssignment([FromBody] IssueAssignmentDto assignmentDto)
        {
            var createdAssignment = await _issueAssignmentService.CreateIssueAssignmentAsync(assignmentDto);
            return CreatedAtAction(nameof(GetIssueAssignment), new { id = createdAssignment.AssignmentId }, createdAssignment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IssueAssignmentDto>> UpdateIssueAssignment(int id, [FromBody] IssueAssignmentDto assignmentDto)
        {
            if (id != assignmentDto.AssignmentId)
                return BadRequest("ID mismatch");

            var updatedAssignment = await _issueAssignmentService.UpdateIssueAssignmentAsync(id, assignmentDto);
            if (updatedAssignment == null)
                return NotFound();

            return Ok(updatedAssignment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssueAssignment(int id)
        {
            var result = await _issueAssignmentService.DeleteIssueAssignmentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}