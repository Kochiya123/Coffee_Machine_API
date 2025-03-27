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
        Task<(IEnumerable<OrderDetailDto> Details, PaginationMetadata Pagination)> GetOrderDetailsAsync(int? orderId, int? productId, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<OrderDetailDto> GetOrderDetailByIdAsync(int id);
        Task<OrderDetailDto> CreateOrderDetailAsync(OrderDetailDto detailDto);
        Task<OrderDetailDto> UpdateOrderDetailAsync(int id, OrderDetailDto detailDto);
        Task<bool> DeleteOrderDetailAsync(int id);
    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public OrderDetailService(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<(IEnumerable<OrderDetailDto> Details, PaginationMetadata Pagination)> GetOrderDetailsAsync(int? orderId, int? productId, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _orderDetailRepository.Query();

            if (orderId.HasValue)
                query = query.Where(d => d.OrderId == orderId);
            if (productId.HasValue)
                query = query.Where(d => d.ProductId == productId);
            if (status.HasValue)
                query = query.Where(d => d.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(OrderDetail).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(d => EF.Property<object>(d, sortBy))
                    : query.OrderByDescending(d => EF.Property<object>(d, sortBy));
            }

            var totalCount = await query.CountAsync();
            var details = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var detailDtos = details.Select(d => new OrderDetailDto
            {
                OrderDetailId = d.OrderDetailId,
                Quantity = d.Quantity,
                Price = d.Price,
                Status = d.Status,
                OrderId = d.OrderId,
                ProductId = d.ProductId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (detailDtos, paginationMetadata);
        }

        public async Task<OrderDetailDto> GetOrderDetailByIdAsync(int id)
        {
           
            var detail = await _orderDetailRepository.GetByIdAsync(id);
            if (detail == null)
                return null;

            return new OrderDetailDto
            {
                OrderDetailId = detail.OrderDetailId,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Status = detail.Status,
                OrderId = detail.OrderId,
                ProductId = detail.ProductId
            };
        }

        public async Task<OrderDetailDto> CreateOrderDetailAsync(OrderDetailDto detailDto)
        {
            var detail = new OrderDetail
            {
                Quantity = detailDto.Quantity,
                Price = detailDto.Price,
                Status = detailDto.Status,
                OrderId = detailDto.OrderId,
                ProductId = detailDto.ProductId
            };

            await _orderDetailRepository.AddAsync(detail);
            await _orderDetailRepository.SaveChangesAsync();
            detailDto.OrderDetailId = detail.OrderDetailId;
            return detailDto;
        }

        public async Task<OrderDetailDto> UpdateOrderDetailAsync(int id, OrderDetailDto detailDto)
        {
            
            var detail = await _orderDetailRepository.GetByIdAsync(id);
            if (detail == null)
                return null;

            detail.Quantity = detailDto.Quantity;
            detail.Price = detailDto.Price;
            detail.Status = detailDto.Status;
            detail.OrderId = detailDto.OrderId;
            detail.ProductId = detailDto.ProductId;

            await _orderDetailRepository.UpdateAsync(detail);
            await _orderDetailRepository.SaveChangesAsync();

            return detailDto;
        }

        public async Task<bool> DeleteOrderDetailAsync(int id)
        {
            
            var detail = await _orderDetailRepository.GetByIdAsync(id);
            if (detail == null)
                return false;

            await _orderDetailRepository.DeleteAsync(id);
            await _orderDetailRepository.SaveChangesAsync();
            return true;
        }
    }
}