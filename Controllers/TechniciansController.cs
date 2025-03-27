using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/technicians")]
    public class TechnicianController : ControllerBase
    {
        private readonly ITechnicianService _technicianService;

        public TechnicianController(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<TechnicianDto>, PaginationMetadata)>> GetTechnicians(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? phoneNumber,
            [FromQuery] string? email,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "TechnicianId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (technicians, pagination) = await _technicianService.GetTechniciansAsync(firstName, lastName, phoneNumber, email, status, sortBy, isAscending, page, pageSize);
            return Ok(new { Technicians = technicians, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TechnicianDto>> GetTechnicianById(int id)
        {
            var technician = await _technicianService.GetTechnicianByIdAsync(id);
            if (technician == null)
                return NotFound();

            return Ok(technician);
        }

        [HttpPost]
        public async Task<ActionResult<TechnicianDto>> CreateTechnician([FromBody] TechnicianDto technicianDto)
        {
            var createdTechnician = await _technicianService.CreateTechnicianAsync(technicianDto);
            return CreatedAtAction(nameof(GetTechnicianById), new { id = createdTechnician.TechnicianId }, createdTechnician);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TechnicianDto>> UpdateTechnician(int id, [FromBody] TechnicianDto technicianDto)
        {
            if (id != technicianDto.TechnicianId)
                return BadRequest("ID mismatch");

            var updatedTechnician = await _technicianService.UpdateTechnicianAsync(id, technicianDto);
            if (updatedTechnician == null)
                return NotFound();

            return Ok(updatedTechnician);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechnician(int id)
        {
            var result = await _technicianService.DeleteTechnicianAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

}