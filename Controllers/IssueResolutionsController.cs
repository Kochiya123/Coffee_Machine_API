using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssueResolutionController : ControllerBase
    {
        private readonly IIssueResolutionService _issueResolutionService;

        public IssueResolutionController(IIssueResolutionService issueResolutionService)
        {
            _issueResolutionService = issueResolutionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIssueResolutions(
            [FromQuery] int? ResolutionId, [FromQuery] DateTime? ResolutionDate, [FromQuery] string? ResolutionDescription,
            [FromQuery] int? Status, [FromQuery] int? IssueId, [FromQuery] int? TechnicianId,
            [FromQuery] string sortBy = "ResolutionId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (issueResolutions, pagination) = await _issueResolutionService.GetIssueResolutionsAsync(ResolutionId, ResolutionDate, ResolutionDescription, Status, IssueId, TechnicianId, sortBy, isAscending, page, pageSize);
            return Ok(new { IssueResolutions = issueResolutions, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssueResolutionById(int id)
        {
            var issueResolution = await _issueResolutionService.GetIssueResolutionByIdAsync(id);
            if (issueResolution == null)
            {
                return NotFound();
            }
            return Ok(issueResolution);
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssueResolution(IssueResolution issueResolution)
        {
            var createdIssueResolution = await _issueResolutionService.CreateIssueResolutionAsync(issueResolution);
            return CreatedAtAction(nameof(GetIssueResolutionById), new { id = createdIssueResolution.ResolutionId }, createdIssueResolution);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssueResolution(int id, IssueResolution issueResolution)
        {
            var updatedIssueResolution = await _issueResolutionService.UpdateIssueResolutionAsync(id, issueResolution);
            if (updatedIssueResolution == null)
            {
                return NotFound();
            }
            return Ok(updatedIssueResolution);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssueResolution(int id)
        {
            var result = await _issueResolutionService.DeleteIssueResolutionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}