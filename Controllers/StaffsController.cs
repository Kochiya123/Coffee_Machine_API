using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/staff")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs(
            [FromQuery] long? StaffId, 
            [FromQuery] string? FirstName, 
            [FromQuery] string? LastName,
            [FromQuery] string? PhoneNumber, 
            [FromQuery] string? Email, 
            [FromQuery] int? Status, 
            [FromQuery] long? StoreId,
            [FromQuery] string sortBy = "StaffId", 
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var (staffs, pagination) = await _staffService.GetStaffsAsync(StaffId, FirstName, LastName, PhoneNumber, Email, Status, StoreId, sortBy, isAscending, page, pageSize);
            return Ok(new { Staffs = staffs, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(long id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(Staff staff)
        {
            var createdStaff = await _staffService.CreateStaffAsync(staff);
            return CreatedAtAction(nameof(GetStaffById), new { id = createdStaff.StaffId }, createdStaff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(long id, Staff staff)
        {
            var updatedStaff = await _staffService.UpdateStaffAsync(id, staff);
            if (updatedStaff == null)
            {
                return NotFound();
            }
            return Ok(updatedStaff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(long id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}