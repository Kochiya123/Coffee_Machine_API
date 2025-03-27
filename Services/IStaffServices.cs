using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IStaffService
    {
        Task<(IEnumerable<StaffDto> Staff, PaginationMetadata Pagination)> GetStaffAsync(string? firstName, string? lastName, string? phoneNumber, string? email, int? status, long? storeId, string sortBy, bool isAscending, int page, int pageSize);
        Task<StaffDto> GetStaffByIdAsync(long id);
        Task<StaffDto> CreateStaffAsync(StaffDto staffDto);
        Task<StaffDto> UpdateStaffAsync(long id, StaffDto staffDto);
        Task<bool> DeleteStaffAsync(long id);
    }

    public class StaffService : IStaffService
    {
        private readonly IRepository<Staff> _staffRepository;

        public StaffService(IRepository<Staff> staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<(IEnumerable<StaffDto> Staff, PaginationMetadata Pagination)> GetStaffAsync(string? firstName, string? lastName, string? phoneNumber, string? email, int? status, long? storeId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _staffRepository.Query();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(s => s.FirstName == firstName);
            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(s => s.LastName == lastName);
            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(s => s.PhoneNumber == phoneNumber);
            if (!string.IsNullOrEmpty(email))
                query = query.Where(s => s.Email == email);
            if (status.HasValue)
                query = query.Where(s => s.Status == status);
            if (storeId.HasValue)
                query = query.Where(s => s.StoreId == storeId);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Staff).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(s => EF.Property<object>(s, sortBy))
                    : query.OrderByDescending(s => EF.Property<object>(s, sortBy));
            }

            var totalCount = await query.CountAsync();
            var staffList = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var staffDtos = staffList.Select(s => new StaffDto
            {
                StaffId = s.StaffId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Status = s.Status,
                StoreId = s.StoreId,
                MachineIssues = s.MachineIssues
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (staffDtos, paginationMetadata);
        }

        public async Task<StaffDto> GetStaffByIdAsync(long id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
                return null;

            return new StaffDto
            {
                StaffId = staff.StaffId,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                PhoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                Status = staff.Status,
                StoreId = staff.StoreId,
                MachineIssues = staff.MachineIssues
            };
        }

        public async Task<StaffDto> CreateStaffAsync(StaffDto staffDto)
        {
            var staff = new Staff
            {
                FirstName = staffDto.FirstName,
                LastName = staffDto.LastName,
                PhoneNumber = staffDto.PhoneNumber,
                Email = staffDto.Email,
                Status = staffDto.Status,
                StoreId = staffDto.StoreId,
                MachineIssues = staffDto.MachineIssues
            };

            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveChangesAsync();
            staffDto.StaffId = staff.StaffId;
            return staffDto;
        }

        public async Task<StaffDto> UpdateStaffAsync(long id, StaffDto staffDto)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
                return null;

            staff.FirstName = staffDto.FirstName;
            staff.LastName = staffDto.LastName;
            staff.PhoneNumber = staffDto.PhoneNumber;
            staff.Email = staffDto.Email;
            staff.Status = staffDto.Status;
            staff.StoreId = staffDto.StoreId;
            staff.MachineIssues = staffDto.MachineIssues;

            await _staffRepository.UpdateAsync(staff);
            await _staffRepository.SaveChangesAsync();

            return staffDto;
        }

        public async Task<bool> DeleteStaffAsync(long id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
                return false;

            await _staffRepository.DeleteAsync(id);
            await _staffRepository.SaveChangesAsync();
            return true;
        }
    }
}