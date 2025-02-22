using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

[Route("api/machineType")]
[ApiController]
public class MachineTypeController : ControllerBase
{
    private readonly CoffeeShop01Context _context;

    public MachineTypeController(CoffeeShop01Context context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MachineType>>> GetMachineTypes()
    {
        return await _context.MachineTypes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MachineType>> GetMachineType(int id)
    {
        var machineType = await _context.MachineTypes.FindAsync(id);

        if (machineType == null)
        {
            return NotFound();
        }

        return machineType;
    }

    [HttpPost]
    public async Task<ActionResult<MachineType>> PostMachineType(MachineType machineType)
    {
        _context.MachineTypes.Add(machineType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMachineType), new { id = machineType.MachineTypeId }, machineType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMachineType(int id, MachineType machineType)
    {
        if (id != machineType.MachineTypeId)
        {
            return BadRequest();
        }

        _context.Entry(machineType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.MachineTypes.Any(e => e.MachineTypeId == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMachineType(int id)
    {
        var machineType = await _context.MachineTypes.FindAsync(id);
        if (machineType == null)
        {
            return NotFound();
        }

        _context.MachineTypes.Remove(machineType);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
