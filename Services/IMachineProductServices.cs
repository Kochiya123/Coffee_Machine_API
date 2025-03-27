using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IMachineProductService
    {
        Task<(IEnumerable<MachineProductDto> Products, PaginationMetadata Pagination)> GetMachineProductsAsync(int? machineId, int? productId, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineProductDto> GetMachineProductByIdAsync(int id);
        Task<MachineProductDto> CreateMachineProductAsync(MachineProductDto productDto);
        Task<MachineProductDto> UpdateMachineProductAsync(int id, MachineProductDto productDto);
        Task<bool> DeleteMachineProductAsync(int id);
    }

    public class MachineProductService : IMachineProductService
    {
        private readonly IRepository<MachineProduct> _productRepository;

        public MachineProductService(IRepository<MachineProduct> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<MachineProductDto> Products, PaginationMetadata Pagination)> GetMachineProductsAsync(int? machineId, int? productId, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _productRepository.Query();

            if (machineId.HasValue)
                query = query.Where(p => p.MachineId == machineId);
            if (productId.HasValue)
                query = query.Where(p => p.ProductId == productId);
            if (status.HasValue)
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineProduct).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                    : query.OrderByDescending(p => EF.Property<object>(p, sortBy));
            }

            var totalCount = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var productDtos = products.Select(p => new MachineProductDto
            {
                MachineProductId = p.MachineProductId,
                MachineStockQuantity = p.MachineStockQuantity,
                Status = p.Status,
                MachineId = p.MachineId,
                ProductId = p.ProductId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (productDtos, paginationMetadata);
        }

        public async Task<MachineProductDto> GetMachineProductByIdAsync(int id)
        {
            
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            return new MachineProductDto
            {
                MachineProductId = product.MachineProductId,
                MachineStockQuantity = product.MachineStockQuantity,
                Status = product.Status,
                MachineId = product.MachineId,
                ProductId = product.ProductId
            };
        }

        public async Task<MachineProductDto> CreateMachineProductAsync(MachineProductDto productDto)
        {
            var product = new MachineProduct
            {
                MachineStockQuantity = productDto.MachineStockQuantity,
                Status = productDto.Status,
                MachineId = productDto.MachineId,
                ProductId = productDto.ProductId
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            productDto.MachineProductId = product.MachineProductId;
            return productDto;
        }

        public async Task<MachineProductDto> UpdateMachineProductAsync(int id, MachineProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            product.MachineStockQuantity = productDto.MachineStockQuantity;
            product.Status = productDto.Status;
            product.MachineId = productDto.MachineId;
            product.ProductId = productDto.ProductId;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return productDto;
        }

        public async Task<bool> DeleteMachineProductAsync(int id)
        {
            
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();
            return true;
        }
    }
}