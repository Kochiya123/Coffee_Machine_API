// ManagerController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/managers")]
    public class ManagerController : ControllerBase
    {
            private readonly IManagerService _managerService;

            public ManagerController(IManagerService managerService)
            {
                _managerService = managerService;
            }

            [HttpGet]
            public async Task<ActionResult<(IEnumerable<ManagerDto>, PaginationMetadata)>> GetManagers(
                [FromQuery] string? username,
                [FromQuery] string? email,
                [FromQuery] string? phoneNumber,
                [FromQuery] int? status,
                [FromQuery] long? storeId,
                [FromQuery] string sortBy = "ManagerId",
                [FromQuery] bool isAscending = true,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
            {
                var (managers, pagination) = await _managerService.GetManagersAsync(username, email, phoneNumber, status, storeId, sortBy, isAscending, page, pageSize);
                return Ok(new { Managers = managers, Pagination = pagination });
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<ManagerDto>> GetManager(long id)
            {
                var manager = await _managerService.GetManagerByIdAsync(id);
                if (manager == null)
                    return NotFound();

                return Ok(manager);
            }

            [HttpPost]
            public async Task<ActionResult<ManagerDto>> CreateManager([FromBody] ManagerDto managerDto)
            {
                var createdManager = await _managerService.CreateManagerAsync(managerDto);
                return CreatedAtAction(nameof(GetManager), new { id = createdManager.ManagerId }, createdManager);
            }

            [HttpPut("{id}")]
            public async Task<ActionResult<ManagerDto>> UpdateManager(long id, [FromBody] ManagerDto managerDto)
            {
                if (id != managerDto.ManagerId)
                    return BadRequest("ID mismatch");

                var updatedManager = await _managerService.UpdateManagerAsync(id, managerDto);
                if (updatedManager == null)
                    return NotFound();

                return Ok(updatedManager);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteManager(long id)
            {
                var result = await _managerService.DeleteManagerAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
        }
    }