using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssueAssignmentController : ControllerBase
    {
        private readonly IIssueAssignmentService _issueAssignmentService;

        public IssueAssignmentController(IIssueAssignmentService issueAssignmentService)
        {
            _issueAssignmentService = issueAssignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIssueAssignments(
            [FromQuery] int? AssignmentId, [FromQuery] DateTime? AssignedDate, [FromQuery] int? Status,
            [FromQuery] int? IssueId, [FromQuery] int? TechnicianId,
            [FromQuery] string sortBy = "AssignmentId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (issueAssignments, pagination) = await _issueAssignmentService.GetIssueAssignmentsAsync(AssignmentId, AssignedDate, Status, IssueId, TechnicianId, sortBy, isAscending, page, pageSize);
            return Ok(new { IssueAssignments = issueAssignments, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssueAssignmentById(int id)
        {
            var issueAssignment = await _issueAssignmentService.GetIssueAssignmentByIdAsync(id);
            if (issueAssignment == null)
            {
                return NotFound();
            }
            return Ok(issueAssignment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssueAssignment(IssueAssignment issueAssignment)
        {
            var createdIssueAssignment = await _issueAssignmentService.CreateIssueAssignmentAsync(issueAssignment);
            return CreatedAtAction(nameof(GetIssueAssignmentById), new { id = createdIssueAssignment.AssignmentId }, createdIssueAssignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssueAssignment(int id, IssueAssignment issueAssignment)
        {
            var updatedIssueAssignment = await _issueAssignmentService.UpdateIssueAssignmentAsync(id, issueAssignment);
            if (updatedIssueAssignment == null)
            {
                return NotFound();
            }
            return Ok(updatedIssueAssignment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssueAssignment(int id)
        {
            var result = await _issueAssignmentService.DeleteIssueAssignmentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}