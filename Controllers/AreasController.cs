using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [Route("api/areas")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<Area>, PaginationMetadata)>> GetAreas(
            [FromQuery] string? areaName,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "AreaId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (areas, pagination) = await _areaService.GetAreasAsync(areaName, status, sortBy, isAscending, page, pageSize);
            return Ok(new { Areas = areas, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> GetArea(int id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
                return NotFound();

            return Ok(area);
        }

        [HttpPost]
        public async Task<ActionResult<Area>> CreateArea([FromBody] Area area)
        {
            var createdArea = await _areaService.CreateAreaAsync(area);
            return CreatedAtAction(nameof(GetArea), new { id = createdArea.AreaId }, createdArea);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Area>> UpdateArea(int id, [FromBody] Area area)
        {
            if (id != area.AreaId)
                return BadRequest("ID mismatch");

            var updatedArea = await _areaService.UpdateAreaAsync(id, area);
            if (updatedArea == null)
                return NotFound();

            return Ok(updatedArea);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var result = await _areaService.DeleteAreaAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}