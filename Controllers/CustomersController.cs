using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services;
using WebApplication2.Models;

[Route("api/customer")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Get customers with optional filtering, sorting, and pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? email,
        [FromQuery] int? status,
        [FromQuery] string? sortBy,
        [FromQuery] bool isAscending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (customers, pagination) = await _customerService.GetCustomersAsync(
            firstName, lastName, email, status, sortBy, isAscending, page, pageSize);

        return Ok(new { Customers = customers, Pagination = pagination });
    }

    /// <summary>
    /// Get a customer by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetCustomerById(long id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound(new { Message = "Customer not found." });

        return Ok(customer);
    }


    /// Create a new customer.

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customerDto)
    {
        if (customerDto == null)
            return BadRequest(new { Message = "Invalid customer data." });

        var createdCustomer = await _customerService.CreateCustomerAsync(customerDto);
        return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId }, createdCustomer);
    }


    /// Update an existing customer.
 
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateCustomer(long id, [FromBody] CustomerDto customerDto)
    {
        if (customerDto == null)
            return BadRequest(new { Message = "Invalid customer data." });

        var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);
        if (updatedCustomer == null)
            return NotFound(new { Message = "Customer not found." });

        return Ok(updatedCustomer);
    }


    /// Delete a customer by ID.

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCustomer(long id)
    {
        var isDeleted = await _customerService.DeleteCustomerAsync(id);
        if (!isDeleted)
            return NotFound(new { Message = "Customer not found." });

        return NoContent();
    }
}
