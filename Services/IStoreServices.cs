using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IStoreService
    {
        Task<(IEnumerable<Store> Stores, PaginationMetadata Pagination)> GetStoresAsync(
            long? StoreId, string? StoreName, string? StoreLocation, string? PhoneNumber, int? Status, int? AreaId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Store> GetStoreByIdAsync(long id);
        Task<Store> CreateStoreAsync(Store store);
        Task<Store> UpdateStoreAsync(long id, Store store);
        Task<bool> DeleteStoreAsync(long id);
    }

    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;

        public StoreService(IRepository<Store> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<(IEnumerable<Store> Stores, PaginationMetadata Pagination)> GetStoresAsync(
            long? StoreId, string? StoreName, string? StoreLocation, string? PhoneNumber, int? Status, int? AreaId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _storeRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (StoreId.HasValue)
            {
                query = query.Where(s => s.StoreId == StoreId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(StoreName))
            {
                query = query.Where(s => s.StoreName.Contains(StoreName));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(StoreLocation))
            {
                query = query.Where(s => s.StoreLocation.Contains(StoreLocation));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                query = query.Where(s => s.PhoneNumber.Contains(PhoneNumber));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(s => s.Status == Status);
                hasFilters = true;
            }
            if (AreaId.HasValue)
            {
                query = query.Where(s => s.AreaId == AreaId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _storeRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Store).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(s => EF.Property<object>(s, sortBy))
                    : query.OrderByDescending(s => EF.Property<object>(s, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var stores = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (stores, paginationMetadata);
        }

        public async Task<Store> GetStoreByIdAsync(long id)
        {
            return await _storeRepository.Query()
                .Where(s => s.StoreId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {
            await _storeRepository.AddAsync(store);
            await _storeRepository.SaveChangesAsync();
            return store;
        }

        public async Task<Store> UpdateStoreAsync(long id, Store store)
        {
            var existingStore = await _storeRepository.GetByIdAsync(id);
            if (existingStore == null)
            {
                return null;
            }

            existingStore.StoreName = store.StoreName;
            existingStore.StoreLocation = store.StoreLocation;
            existingStore.PhoneNumber = store.PhoneNumber;
            existingStore.Status = store.Status;
            existingStore.AreaId = store.AreaId;

            await _storeRepository.UpdateAsync(existingStore);
            await _storeRepository.SaveChangesAsync();

            return existingStore;
        }

        public async Task<bool> DeleteStoreAsync(long id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
            {
                return false;
            }

            await _storeRepository.DeleteAsync(id);
            await _storeRepository.SaveChangesAsync();

            return true;
        }
    }
}