using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IOrderService
    {
        Task<(IEnumerable<OrderDto> Orders, PaginationMetadata Pagination)> GetOrdersAsync(
            int? OrderId, DateTime? OrderDate, string? OrderDescription, decimal? TotalAmount, int? status,long? CustomerId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
        Task<OrderDto> UpdateOrderAsync(int id, OrderDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<(IEnumerable<OrderDto> Orders, PaginationMetadata Pagination)> GetOrdersAsync(
            int? OrderId, DateTime? OrderDate, string? OrderDescription, decimal? TotalAmount, int? status,long? CustomerId,
            string sortBy, bool isAscending, int page, int pageSize)

        {
            var query = _orderRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            if (CustomerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == CustomerId);
                hasFilters = true;
            }
            if (OrderDate.HasValue)
            {
                query = query.Where(o => o.OrderDate == OrderDate);
                hasFilters = true;
            }
            if (TotalAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount == TotalAmount);
                hasFilters = true;
            }
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _orderRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Order).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(o => EF.Property<object>(o, sortBy))
                    : query.OrderByDescending(o => EF.Property<object>(o, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var orders = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Map to DTOs
            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                OrderDescription = o.OrderDescription,
                TotalAmount = o.TotalAmount,
                CustomerId = o.CustomerId,
                Status = o.Status,
            });

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (orderDtos, paginationMetadata);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.Query()
                .Where(o => o.OrderId == id)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderDescription = order.OrderDescription,
                TotalAmount = order.TotalAmount,
                CustomerId = order.CustomerId,
                Status = order.Status,
            };
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            var order = new Order
            {
                OrderDate = orderDto.OrderDate,
                OrderDescription = orderDto.OrderDescription,
                TotalAmount = orderDto.TotalAmount,
                Status = orderDto.Status,
                CustomerId = 1, // Replace with actual logic to set CustomerId
                MachineId = 1,  // Replace with actual logic to set MachineId
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            orderDto.OrderId = order.OrderId; // Set the generated ID
            return orderDto;
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, OrderDto orderDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }

            order.OrderDate = orderDto.OrderDate;
            order.OrderDescription = orderDto.OrderDescription;
            order.TotalAmount = orderDto.TotalAmount;
            order.Status = orderDto.Status;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return orderDto;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return false;
            }

            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();

            return true;
        }
    }

    public class PaginationMetadata
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}