// Services/ICustomerService.cs
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Linq.Expressions;
using WebApplication2.Models;


namespace WebApplication2.Services;

public interface ICustomerService
{
    Task<(IEnumerable<CustomerDto> Customers, PaginationMetadata Pagination)> GetCustomersAsync(
        string firstName, string lastName, string email, int? status,
        string sortBy, bool isAscending, int page, int pageSize);
    // New CRUD methods
    Task<CustomerDto> GetCustomerByIdAsync(long id);
    Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto);
    Task<CustomerDto> UpdateCustomerAsync(long id, CustomerDto customerDto);
    Task<bool> DeleteCustomerAsync(long id);
}

// Services/CustomerService.cs
public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _customerRepository;

    public CustomerService(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    // Existing method for filtering, sorting, and paging
    public async Task<(IEnumerable<CustomerDto> Customers, PaginationMetadata Pagination)> GetCustomersAsync(
    string firstName, string lastName, string email, int? status,
    string sortBy, bool isAscending, int page, int pageSize)
    {
        var query = _customerRepository.Query(); // ✅ Start with IQueryable

        // Apply filtering *only* if the corresponding parameter has a value
        bool hasFilters = false;

        if (!string.IsNullOrEmpty(firstName))
        {
            query = query.Where(c => c.FirstName.Contains(firstName));
            hasFilters = true;
        }
        if (!string.IsNullOrEmpty(lastName))
        {
            query = query.Where(c => c.LastName.Contains(lastName));
            hasFilters = true;
        }
        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(c => c.Email.Contains(email));
            hasFilters = true;
        }
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status);
            hasFilters = true;
        }

        // ✅ If no filters were applied, return all customers without validation errors
        if (!hasFilters)
        {
            query = _customerRepository.Query(); // Reset query to fetch all records
        }

        // Validate and apply sorting
        if (!string.IsNullOrEmpty(sortBy) && typeof(Customer).GetProperty(sortBy) != null)
        {
            query = isAscending
                ? query.OrderBy(c => EF.Property<object>(c, sortBy))
                : query.OrderByDescending(c => EF.Property<object>(c, sortBy));
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var customers = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Map to DTOs
        var customerDtos = customers.Select(c => new CustomerDto
        {
            CustomerId = c.CustomerId,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            Description = c.Description,
            Status = c.Status
        });

        // Create pagination metadata
        var paginationMetadata = new PaginationMetadata
        {
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return (customerDtos, paginationMetadata);
    }


    // Get a single customer by ID
    public async Task<CustomerDto> GetCustomerByIdAsync(long id)
    {
        var customer = await _customerRepository.Query()
        .Where(c => c.CustomerId == id)
        .FirstOrDefaultAsync();

        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            Description = customer.Description,
            Status = customer.Status,

        };
    }

    // Create a new customer
    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
    {
        var customer = new Customer
        {
            FirstName = customerDto.FirstName,
            LastName = customerDto.LastName,
            Email = customerDto.Email,
            PhoneNumber = customerDto.PhoneNumber,
            Address = customerDto.Address,
            Description = customerDto.Description,
            Status = customerDto.Status
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        customerDto.CustomerId = customer.CustomerId; // Set the generated ID
        return customerDto;
    }

    // Update an existing customer
    public async Task<CustomerDto> UpdateCustomerAsync(long id, CustomerDto customerDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return null;
        }

        customer.FirstName = customerDto.FirstName;
        customer.LastName = customerDto.LastName;
        customer.Email = customerDto.Email;
        customer.PhoneNumber = customerDto.PhoneNumber;
        customer.Address = customerDto.Address;
        customer.Description = customerDto.Description;
        customer.Status = customerDto.Status;

        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return customerDto;
    }

    // Delete a customer by ID
    public async Task<bool> DeleteCustomerAsync(long id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return false;
        }

        await _customerRepository.DeleteAsync(id);
        await _customerRepository.SaveChangesAsync();

        return true;
    }
}