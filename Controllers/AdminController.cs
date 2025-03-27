// AdminController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<Admin>, PaginationMetadata)>> GetAdmins(
            [FromQuery] string? username,
            [FromQuery] string? email,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "AdminId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (admins, pagination) = await _adminService.GetAdminsAsync(username, email, status, sortBy, isAscending, page, pageSize);
            return Ok(new { Admins = admins, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(long id)
        {
            var admin = await _adminService.GetAdminByIdAsync(id);
            if (admin == null)
                return NotFound();

            return Ok(admin);
        }

        [HttpPost]
        public async Task<ActionResult<Admin>> CreateAdmin([FromBody] Admin admin)
        {
            var createdAdmin = await _adminService.CreateAdminAsync(admin);
            return CreatedAtAction(nameof(GetAdmin), new { id = createdAdmin.AdminId }, createdAdmin);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Admin>> UpdateAdmin(long id, [FromBody] Admin admin)
        {
            if (id != admin.AdminId)
                return BadRequest("ID mismatch");

            var updatedAdmin = await _adminService.UpdateAdminAsync(id, admin);
            if (updatedAdmin == null)
                return NotFound();

            return Ok(updatedAdmin);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(long id)
        {
            var result = await _adminService.DeleteAdminAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}