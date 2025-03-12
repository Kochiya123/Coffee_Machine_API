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
        Task<(IEnumerable<MachineProduct> MachineProducts, PaginationMetadata Pagination)> GetMachineProductsAsync(
            int? MachineProductId, int? MachineStockQuantity, int? Status, int? MachineId, int? ProductId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineProduct> GetMachineProductByIdAsync(int id);
        Task<MachineProduct> CreateMachineProductAsync(MachineProduct machineProduct);
        Task<MachineProduct> UpdateMachineProductAsync(int id, MachineProduct machineProduct);
        Task<bool> DeleteMachineProductAsync(int id);
    }

    public class MachineProductService : IMachineProductService
    {
        private readonly IRepository<MachineProduct> _machineProductRepository;

        public MachineProductService(IRepository<MachineProduct> machineProductRepository)
        {
            _machineProductRepository = machineProductRepository;
        }

        public async Task<(IEnumerable<MachineProduct> MachineProducts, PaginationMetadata Pagination)> GetMachineProductsAsync(
            int? MachineProductId, int? MachineStockQuantity, int? Status, int? MachineId, int? ProductId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineProductRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (MachineProductId.HasValue)
            {
                query = query.Where(mp => mp.MachineProductId == MachineProductId);
                hasFilters = true;
            }
            if (MachineStockQuantity.HasValue)
            {
                query = query.Where(mp => mp.MachineStockQuantity == MachineStockQuantity);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(mp => mp.Status == Status);
                hasFilters = true;
            }
            if (MachineId.HasValue)
            {
                query = query.Where(mp => mp.MachineId == MachineId);
                hasFilters = true;
            }
            if (ProductId.HasValue)
            {
                query = query.Where(mp => mp.ProductId == ProductId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _machineProductRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineProduct).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(mp => EF.Property<object>(mp, sortBy))
                    : query.OrderByDescending(mp => EF.Property<object>(mp, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var machineProducts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (machineProducts, paginationMetadata);
        }

        public async Task<MachineProduct> GetMachineProductByIdAsync(int id)
        {
            return await _machineProductRepository.Query()
                .Where(mp => mp.MachineProductId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MachineProduct> CreateMachineProductAsync(MachineProduct machineProduct)
        {
            await _machineProductRepository.AddAsync(machineProduct);
            await _machineProductRepository.SaveChangesAsync();
            return machineProduct;
        }

        public async Task<MachineProduct> UpdateMachineProductAsync(int id, MachineProduct machineProduct)
        {
            var existingMachineProduct = await _machineProductRepository.GetByIdAsync(id);
            if (existingMachineProduct == null)
            {
                return null;
            }

            existingMachineProduct.MachineStockQuantity = machineProduct.MachineStockQuantity;
            existingMachineProduct.Status = machineProduct.Status;
            existingMachineProduct.MachineId = machineProduct.MachineId;
            existingMachineProduct.ProductId = machineProduct.ProductId;

            await _machineProductRepository.UpdateAsync(existingMachineProduct);
            await _machineProductRepository.SaveChangesAsync();

            return existingMachineProduct;
        }

        public async Task<bool> DeleteMachineProductAsync(int id)
        {
            var machineProduct = await _machineProductRepository.GetByIdAsync(id);
            if (machineProduct == null)
            {
                return false;
            }

            await _machineProductRepository.DeleteAsync(id);
            await _machineProductRepository.SaveChangesAsync();

            return true;
        }
    }
}