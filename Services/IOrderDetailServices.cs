using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IOrderDetailService
    {
        Task<(IEnumerable<OrderDetail> OrderDetails, PaginationMetadata Pagination)> GetOrderDetailsAsync(
            int? OrderDetailId, int? Quantity, decimal? Price, int? Status, int? OrderId, int? ProductId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<OrderDetail> GetOrderDetailByIdAsync(int id);
        Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail> UpdateOrderDetailAsync(int id, OrderDetail orderDetail);
        Task<bool> DeleteOrderDetailAsync(int id);
    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public OrderDetailService(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<(IEnumerable<OrderDetail> OrderDetails, PaginationMetadata Pagination)> GetOrderDetailsAsync(
            int? OrderDetailId, int? Quantity, decimal? Price, int? Status, int? OrderId, int? ProductId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _orderDetailRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (OrderDetailId.HasValue)
            {
                query = query.Where(od => od.OrderDetailId == OrderDetailId);
                hasFilters = true;
            }
            if (Quantity.HasValue)
            {
                query = query.Where(od => od.Quantity == Quantity);
                hasFilters = true;
            }
            if (Price.HasValue)
            {
                query = query.Where(od => od.Price == Price);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(od => od.Status == Status);
                hasFilters = true;
            }
            if (OrderId.HasValue)
            {
                query = query.Where(od => od.OrderId == OrderId);
                hasFilters = true;
            }
            if (ProductId.HasValue)
            {
                query = query.Where(od => od.ProductId == ProductId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _orderDetailRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(OrderDetail).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(od => EF.Property<object>(od, sortBy))
                    : query.OrderByDescending(od => EF.Property<object>(od, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var orderDetails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (orderDetails, paginationMetadata);
        }

        public async Task<OrderDetail> GetOrderDetailByIdAsync(int id)
        {
            return await _orderDetailRepository.Query()
                .Where(od => od.OrderDetailId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            await _orderDetailRepository.AddAsync(orderDetail);
            await _orderDetailRepository.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail> UpdateOrderDetailAsync(int id, OrderDetail orderDetail)
        {
            var existingOrderDetail = await _orderDetailRepository.GetByIdAsync(id);
            if (existingOrderDetail == null)
            {
                return null;
            }

            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Price = orderDetail.Price;
            existingOrderDetail.Status = orderDetail.Status;
            existingOrderDetail.OrderId = orderDetail.OrderId;
            existingOrderDetail.ProductId = orderDetail.ProductId;

            await _orderDetailRepository.UpdateAsync(existingOrderDetail);
            await _orderDetailRepository.SaveChangesAsync();

            return existingOrderDetail;
        }

        public async Task<bool> DeleteOrderDetailAsync(int id)
        {
            var orderDetail = await _orderDetailRepository.GetByIdAsync(id);
            if (orderDetail == null)
            {
                return false;
            }

            await _orderDetailRepository.DeleteAsync(id);
            await _orderDetailRepository.SaveChangesAsync();

            return true;
        }
    }
}