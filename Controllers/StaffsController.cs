using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/staffs")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<StaffDto>, PaginationMetadata)>> GetStaff(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? phoneNumber,
            [FromQuery] string? email,
            [FromQuery] int? status,
            [FromQuery] long? storeId,
            [FromQuery] string sortBy = "StaffId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (staff, pagination) = await _staffService.GetStaffAsync(firstName, lastName, phoneNumber, email, status, storeId, sortBy, isAscending, page, pageSize);
            return Ok(new { Staff = staff, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetStaffById(long id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }

        [HttpPost]
        public async Task<ActionResult<StaffDto>> CreateStaff([FromBody] StaffDto staffDto)
        {
            var createdStaff = await _staffService.CreateStaffAsync(staffDto);
            return CreatedAtAction(nameof(GetStaffById), new { id = createdStaff.StaffId }, createdStaff);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StaffDto>> UpdateStaff(long id, [FromBody] StaffDto staffDto)
        {
            if (id != staffDto.StaffId)
                return BadRequest("ID mismatch");

            var updatedStaff = await _staffService.UpdateStaffAsync(id, staffDto);
            if (updatedStaff == null)
                return NotFound();

            return Ok(updatedStaff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(long id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}