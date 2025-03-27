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
        Task<(IEnumerable<StoreDto> Stores, PaginationMetadata Pagination)> GetStoresAsync(string? storeName, string? storeLocation, string? phoneNumber, int? status, int? areaId, string sortBy, bool isAscending, int page, int pageSize);
        Task<StoreDto> GetStoreByIdAsync(long id);
        Task<StoreDto> CreateStoreAsync(StoreDto storeDto);
        Task<StoreDto> UpdateStoreAsync(long id, StoreDto storeDto);
        Task<bool> DeleteStoreAsync(long id);
    }

    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;

        public StoreService(IRepository<Store> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<(IEnumerable<StoreDto> Stores, PaginationMetadata Pagination)> GetStoresAsync(string? storeName, string? storeLocation, string? phoneNumber, int? status, int? areaId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _storeRepository.Query();

            if (!string.IsNullOrEmpty(storeName))
                query = query.Where(s => s.StoreName.Contains(storeName));
            if (!string.IsNullOrEmpty(storeLocation))
                query = query.Where(s => s.StoreLocation.Contains(storeLocation));
            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(s => s.PhoneNumber.Contains(phoneNumber));
            if (status.HasValue)
                query = query.Where(s => s.Status == status);
            if (areaId.HasValue)
                query = query.Where(s => s.AreaId == areaId);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Store).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(s => EF.Property<object>(s, sortBy))
                    : query.OrderByDescending(s => EF.Property<object>(s, sortBy));
            }

            var totalCount = await query.CountAsync();
            var stores = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var storeDtos = stores.Select(s => new StoreDto
            {
                StoreId = s.StoreId,
                StoreName = s.StoreName,
                StoreLocation = s.StoreLocation,
                PhoneNumber = s.PhoneNumber,
                Status = s.Status,
                AreaId = s.AreaId,
                Machines = s.Machines,
                Managers = s.Managers,
                Staff = s.Staff
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (storeDtos, paginationMetadata);
        }

        public async Task<StoreDto> GetStoreByIdAsync(long id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
                return null;

            return new StoreDto
            {
                StoreId = store.StoreId,
                StoreName = store.StoreName,
                StoreLocation = store.StoreLocation,
                PhoneNumber = store.PhoneNumber,
                Status = store.Status,
                AreaId = store.AreaId,
                Machines = store.Machines,
                Managers = store.Managers,
                Staff = store.Staff
            };
        }

        public async Task<StoreDto> CreateStoreAsync(StoreDto storeDto)
        {
            var store = new Store
            {
                StoreName = storeDto.StoreName,
                StoreLocation = storeDto.StoreLocation,
                PhoneNumber = storeDto.PhoneNumber,
                Status = storeDto.Status,
                AreaId = storeDto.AreaId,
                Machines = storeDto.Machines,
                Managers = storeDto.Managers,
                Staff = storeDto.Staff
            };

            await _storeRepository.AddAsync(store);
            await _storeRepository.SaveChangesAsync();
            storeDto.StoreId = store.StoreId;
            return storeDto;
        }

        public async Task<StoreDto> UpdateStoreAsync(long id, StoreDto storeDto)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
                return null;

            store.StoreName = storeDto.StoreName;
            store.StoreLocation = storeDto.StoreLocation;
            store.PhoneNumber = storeDto.PhoneNumber;
            store.Status = storeDto.Status;
            store.AreaId = storeDto.AreaId;
            store.Machines = storeDto.Machines;
            store.Managers = storeDto.Managers;
            store.Staff = storeDto.Staff;

            await _storeRepository.UpdateAsync(store);
            await _storeRepository.SaveChangesAsync();

            return storeDto;
        }

        public async Task<bool> DeleteStoreAsync(long id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
                return false;

            await _storeRepository.DeleteAsync(id);
            await _storeRepository.SaveChangesAsync();
            return true;
        }
    }
}