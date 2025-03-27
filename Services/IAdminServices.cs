// IAdminService.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IAdminService
    {
        Task<(IEnumerable<Admin> Admins, PaginationMetadata Pagination)> GetAdminsAsync(string? username, string? email, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<Admin> GetAdminByIdAsync(long id);
        Task<Admin> CreateAdminAsync(Admin admin);
        Task<Admin> UpdateAdminAsync(long id, Admin admin);
        Task<bool> DeleteAdminAsync(long id);
    }

    public class AdminService : IAdminService
    {
        private readonly IRepository<Admin> _adminRepository;

        public AdminService(IRepository<Admin> adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<(IEnumerable<Admin> Admins, PaginationMetadata Pagination)> GetAdminsAsync(string? username, string? email, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _adminRepository.Query();

            if (!string.IsNullOrEmpty(username))
                query = query.Where(a => a.Username.Contains(username));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(a => a.Email.Contains(email));
            if (status.HasValue)
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Admin).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(a => EF.Property<object>(a, sortBy))
                    : query.OrderByDescending(a => EF.Property<object>(a, sortBy));
            }

            var totalCount = await query.CountAsync();
            var admins = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (admins, paginationMetadata);
        }

        public async Task<Admin> GetAdminByIdAsync(long id)
        {
            return await _adminRepository.GetByIdAsync(id);
        }

        public async Task<Admin> CreateAdminAsync(Admin admin)
        {
            await _adminRepository.AddAsync(admin);
            await _adminRepository.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin> UpdateAdminAsync(long id, Admin admin)
        {
            var existingAdmin = await _adminRepository.GetByIdAsync(id);
            if (existingAdmin == null)
                return null;

            existingAdmin.Username = admin.Username;
            existingAdmin.FirstName = admin.FirstName;
            existingAdmin.LastName = admin.LastName;
            existingAdmin.Email = admin.Email;
            existingAdmin.PhoneNumber = admin.PhoneNumber;
            existingAdmin.Status = admin.Status;

            await _adminRepository.UpdateAsync(existingAdmin);
            await _adminRepository.SaveChangesAsync();
            return existingAdmin;
        }

        public async Task<bool> DeleteAdminAsync(long id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
                return false;

            await _adminRepository.DeleteAsync(id);
            await _adminRepository.SaveChangesAsync();
            return true;
        }
    }
}