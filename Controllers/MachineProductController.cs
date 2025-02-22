using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

[Route("api/machineProduct")]
[ApiController]
public class MachineProductController : ControllerBase
{
    private readonly CoffeeShop01Context _context;

    public MachineProductController(CoffeeShop01Context context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MachineProduct>>> GetMachineProducts()
    {
        return await _context.MachineProducts.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MachineProduct>> GetMachineProduct(int id)
    {
        var machineProduct = await _context.MachineProducts.FindAsync(id);

        if (machineProduct == null)
        {
            return NotFound();
        }

        return machineProduct;
    }

    [HttpPost]
    public async Task<ActionResult<MachineProduct>> PostMachineProduct(MachineProduct machineProduct)
    {
        _context.MachineProducts.Add(machineProduct);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMachineProduct), new { id = machineProduct.MachineProductId }, machineProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMachineProduct(int id, MachineProduct machineProduct)
    {
        if (id != machineProduct.MachineProductId)
        {
            return BadRequest();
        }

        _context.Entry(machineProduct).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.MachineProducts.Any(e => e.MachineProductId == id))
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
    public async Task<IActionResult> DeleteMachineProduct(int id)
    {
        var machineProduct = await _context.MachineProducts.FindAsync(id);
        if (machineProduct == null)
        {
            return NotFound();
        }

        _context.MachineProducts.Remove(machineProduct);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
