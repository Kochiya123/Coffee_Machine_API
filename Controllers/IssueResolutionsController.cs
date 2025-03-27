using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/issue/{issueId}/resolution")]
    public class IssueResolutionController : ControllerBase
    {
        private readonly IIssueResolutionService _issueResolutionService;

        public IssueResolutionController(IIssueResolutionService issueResolutionService)
        {
            _issueResolutionService = issueResolutionService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<IssueResolutionDto>, PaginationMetadata)>> GetIssueResolutions(
            [FromQuery] int? issueId,
            [FromQuery] int? technicianId,
            [FromQuery] int? status,
            [FromQuery] DateTime? resolutionDate,
            [FromQuery] string sortBy = "ResolutionId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (resolutions, pagination) = await _issueResolutionService.GetIssueResolutionsAsync(issueId, technicianId, status, resolutionDate, sortBy, isAscending, page, pageSize);
            return Ok(new { Resolutions = resolutions, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IssueResolutionDto>> GetIssueResolution(int id)
        {
            var resolution = await _issueResolutionService.GetIssueResolutionByIdAsync(id);
            if (resolution == null)
                return NotFound();

            return Ok(resolution);
        }

        [HttpPost]
        public async Task<ActionResult<IssueResolutionDto>> CreateIssueResolution([FromBody] IssueResolutionDto resolutionDto)
        {
            var createdResolution = await _issueResolutionService.CreateIssueResolutionAsync(resolutionDto);
            return CreatedAtAction(nameof(GetIssueResolution), new { id = createdResolution.ResolutionId }, createdResolution);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IssueResolutionDto>> UpdateIssueResolution(int id, [FromBody] IssueResolutionDto resolutionDto)
        {
            if (id != resolutionDto.ResolutionId)
                return BadRequest("ID mismatch");

            var updatedResolution = await _issueResolutionService.UpdateIssueResolutionAsync(id, resolutionDto);
            if (updatedResolution == null)
                return NotFound();

            return Ok(updatedResolution);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssueResolution(int id)
        {
            var result = await _issueResolutionService.DeleteIssueResolutionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

}