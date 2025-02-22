using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

[Route("api/area")]
[ApiController]
public class AreaController : ControllerBase
{
    private readonly CoffeeShop01Context _context;

    public AreaController(CoffeeShop01Context context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
    {
        return await _context.Areas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Area>> GetAreas(int id)
    {
        var area = await _context.Areas.FindAsync(id);

        if (area == null)
        {
            return NotFound();
        }

        return area;
    }

    [HttpPost]
    public async Task<ActionResult<Area>> PostArea(Area area)
    {
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAreas), new { id = area.AreaId }, area);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutArea(int id, Area area)
    {
        if (id != area.AreaId)
        {
            return BadRequest();
        }

        _context.Entry(area).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Areas.Any(e => e.AreaId == id))
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
    public async Task<IActionResult> DeleteArea(int id)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area == null)
        {
            return NotFound();
        }

        _context.Areas.Remove(area);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
