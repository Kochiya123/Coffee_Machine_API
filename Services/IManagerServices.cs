// IManagerService.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IManagerService
    {
        Task<(IEnumerable<ManagerDto> Managers, PaginationMetadata Pagination)> GetManagersAsync(string? username, string? email, string? phoneNumber, int? status, long? storeId, string sortBy, bool isAscending, int page, int pageSize);
        Task<ManagerDto> GetManagerByIdAsync(long id);
        Task<ManagerDto> CreateManagerAsync(ManagerDto managerDto);
        Task<ManagerDto> UpdateManagerAsync(long id, ManagerDto managerDto);
        Task<bool> DeleteManagerAsync(long id);
    }

    public class ManagerService : IManagerService
    {
        private readonly IRepository<Manager> _managerRepository;

        public ManagerService(IRepository<Manager> managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public async Task<(IEnumerable<ManagerDto> Managers, PaginationMetadata Pagination)> GetManagersAsync(string? username, string? email, string? phoneNumber, int? status, long? storeId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _managerRepository.Query();

            if (!string.IsNullOrEmpty(username))
                query = query.Where(m => m.Username == username);
            if (!string.IsNullOrEmpty(email))
                query = query.Where(m => m.Email == email);
            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(m => m.PhoneNumber == phoneNumber);
            if (status.HasValue)
                query = query.Where(m => m.Status == status);
            if (storeId.HasValue)
                query = query.Where(m => m.StoreId == storeId);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Manager).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(m => EF.Property<object>(m, sortBy))
                    : query.OrderByDescending(m => EF.Property<object>(m, sortBy));
            }

            var totalCount = await query.CountAsync();
            var managers = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var managerDtos = managers.Select(m => new ManagerDto
            {
                ManagerId = m.ManagerId,
                Username = m.Username,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                CreatedAt = m.CreatedAt,
                LastLogin = m.LastLogin,
                Status = m.Status,
                StoreId = m.StoreId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (managerDtos, paginationMetadata);
        }

        public async Task<ManagerDto> GetManagerByIdAsync(long id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return null;

            return new ManagerDto
            {
                ManagerId = manager.ManagerId,
                Username = manager.Username,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Email = manager.Email,
                PhoneNumber = manager.PhoneNumber,
                CreatedAt = manager.CreatedAt,
                LastLogin = manager.LastLogin,
                Status = manager.Status,
                StoreId = manager.StoreId
            };
        }

        public async Task<ManagerDto> CreateManagerAsync(ManagerDto managerDto)
        {
            var manager = new Manager
            {
                Username = managerDto.Username,
                FirstName = managerDto.FirstName,
                LastName = managerDto.LastName,
                Email = managerDto.Email,
                PhoneNumber = managerDto.PhoneNumber,
                CreatedAt = managerDto.CreatedAt,
                LastLogin = managerDto.LastLogin,
                Status = managerDto.Status,
                StoreId = (managerDto.StoreId == 0) ? null : managerDto.StoreId
            };

            await _managerRepository.AddAsync(manager);
            await _managerRepository.SaveChangesAsync();
            managerDto.ManagerId = manager.ManagerId;
            return managerDto;
        }

        public async Task<ManagerDto> UpdateManagerAsync(long id, ManagerDto managerDto)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return null;

            manager.Username = managerDto.Username;
            manager.FirstName = managerDto.FirstName;
            manager.LastName = managerDto.LastName;
            manager.Email = managerDto.Email;
            manager.PhoneNumber = managerDto.PhoneNumber;
            manager.Status = managerDto.Status;
            manager.StoreId = (managerDto.StoreId == 0) ? null : managerDto.StoreId;
            manager.LastLogin = managerDto.LastLogin;

            await _managerRepository.UpdateAsync(manager);
            await _managerRepository.SaveChangesAsync();

            return managerDto;
        }

        public async Task<bool> DeleteManagerAsync(long id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return false;

            await _managerRepository.DeleteAsync(id);
            await _managerRepository.SaveChangesAsync();
            return true;
        }
    }
}