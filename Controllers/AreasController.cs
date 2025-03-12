using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/area")]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAreas(
            [FromQuery] int? AreaId, 
            [FromQuery] string? AreaName, 
            [FromQuery] int? Status,
            [FromQuery] string sortBy = "AreaId", 
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var (areas, pagination) = await _areaService.GetAreasAsync(AreaId, AreaName, Status, sortBy, isAscending, page, pageSize);
            return Ok(new { Areas = areas, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return Ok(area);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea(Area area)
        {
            var createdArea = await _areaService.CreateAreaAsync(area);
            return CreatedAtAction(nameof(GetAreaById), new { id = createdArea.AreaId }, createdArea);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, Area area)
        {
            var updatedArea = await _areaService.UpdateAreaAsync(id, area);
            if (updatedArea == null)
            {
                return NotFound();
            }
            return Ok(updatedArea);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var result = await _areaService.DeleteAreaAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}