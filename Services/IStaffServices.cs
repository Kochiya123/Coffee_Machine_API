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
        Task<(IEnumerable<Staff> Staffs, PaginationMetadata Pagination)> GetStaffsAsync(
            long? StaffId, string? FirstName, string? LastName, string? PhoneNumber, string? Email, int? Status, long? StoreId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Staff> GetStaffByIdAsync(long id);
        Task<Staff> CreateStaffAsync(Staff staff);
        Task<Staff> UpdateStaffAsync(long id, Staff staff);
        Task<bool> DeleteStaffAsync(long id);
    }

    public class StaffService : IStaffService
    {
        private readonly IRepository<Staff> _staffRepository;

        public StaffService(IRepository<Staff> staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<(IEnumerable<Staff> Staffs, PaginationMetadata Pagination)> GetStaffsAsync(
            long? StaffId, string? FirstName, string? LastName, string? PhoneNumber, string? Email, int? Status, long? StoreId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _staffRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (StaffId.HasValue)
            {
                query = query.Where(s => s.StaffId == StaffId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(FirstName))
            {
                query = query.Where(s => s.FirstName.Contains(FirstName));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                query = query.Where(s => s.LastName.Contains(LastName));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                query = query.Where(s => s.PhoneNumber.Contains(PhoneNumber));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(s => s.Email.Contains(Email));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(s => s.Status == Status);
                hasFilters = true;
            }
            if (StoreId.HasValue)
            {
                query = query.Where(s => s.StoreId == StoreId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _staffRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Staff).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(s => EF.Property<object>(s, sortBy))
                    : query.OrderByDescending(s => EF.Property<object>(s, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var staffs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (staffs, paginationMetadata);
        }

        public async Task<Staff> GetStaffByIdAsync(long id)
        {
            return await _staffRepository.Query()
                .Where(s => s.StaffId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff> CreateStaffAsync(Staff staff)
        {
            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveChangesAsync();
            return staff;
        }

        public async Task<Staff> UpdateStaffAsync(long id, Staff staff)
        {
            var existingStaff = await _staffRepository.GetByIdAsync(id);
            if (existingStaff == null)
            {
                return null;
            }

            existingStaff.FirstName = staff.FirstName;
            existingStaff.LastName = staff.LastName;
            existingStaff.PhoneNumber = staff.PhoneNumber;
            existingStaff.Email = staff.Email;
            existingStaff.Status = staff.Status;
            existingStaff.StoreId = staff.StoreId;

            await _staffRepository.UpdateAsync(existingStaff);
            await _staffRepository.SaveChangesAsync();

            return existingStaff;
        }

        public async Task<bool> DeleteStaffAsync(long id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
            {
                return false;
            }

            await _staffRepository.DeleteAsync(id);
            await _staffRepository.SaveChangesAsync();

            return true;
        }
    }
}