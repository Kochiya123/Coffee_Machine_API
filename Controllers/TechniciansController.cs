using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechnicianController : ControllerBase
    {
        private readonly ITechnicianService _technicianService;

        public TechnicianController(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTechnicians(
            [FromQuery] int? TechnicianId, [FromQuery] string? FirstName, [FromQuery] string? LastName,
            [FromQuery] string? PhoneNumber, [FromQuery] string? Email, [FromQuery] int? Status,
            [FromQuery] string sortBy = "TechnicianId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (technicians, pagination) = await _technicianService.GetTechniciansAsync(TechnicianId, FirstName, LastName, PhoneNumber, Email, Status, sortBy, isAscending, page, pageSize);
            return Ok(new { Technicians = technicians, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTechnicianById(int id)
        {
            var technician = await _technicianService.GetTechnicianByIdAsync(id);
            if (technician == null)
            {
                return NotFound();
            }
            return Ok(technician);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTechnician(Technician technician)
        {
            var createdTechnician = await _technicianService.CreateTechnicianAsync(technician);
            return CreatedAtAction(nameof(GetTechnicianById), new { id = createdTechnician.TechnicianId }, createdTechnician);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTechnician(int id, Technician technician)
        {
            var updatedTechnician = await _technicianService.UpdateTechnicianAsync(id, technician);
            if (updatedTechnician == null)
            {
                return NotFound();
            }
            return Ok(updatedTechnician);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechnician(int id)
        {
            var result = await _technicianService.DeleteTechnicianAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}