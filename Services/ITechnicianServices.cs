using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface ITechnicianService
    {
        Task<(IEnumerable<Technician> Technicians, PaginationMetadata Pagination)> GetTechniciansAsync(
            int? TechnicianId, string? FirstName, string? LastName, string? PhoneNumber, string? Email, int? Status,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Technician> GetTechnicianByIdAsync(int id);
        Task<Technician> CreateTechnicianAsync(Technician technician);
        Task<Technician> UpdateTechnicianAsync(int id, Technician technician);
        Task<bool> DeleteTechnicianAsync(int id);
    }

    public class TechnicianService : ITechnicianService
    {
        private readonly IRepository<Technician> _technicianRepository;

        public TechnicianService(IRepository<Technician> technicianRepository)
        {
            _technicianRepository = technicianRepository;
        }

        public async Task<(IEnumerable<Technician> Technicians, PaginationMetadata Pagination)> GetTechniciansAsync(
            int? TechnicianId, string? FirstName, string? LastName, string? PhoneNumber, string? Email, int? Status,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _technicianRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (TechnicianId.HasValue)
            {
                query = query.Where(t => t.TechnicianId == TechnicianId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(FirstName))
            {
                query = query.Where(t => t.FirstName.Contains(FirstName));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                query = query.Where(t => t.LastName.Contains(LastName));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                query = query.Where(t => t.PhoneNumber.Contains(PhoneNumber));
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(t => t.Email.Contains(Email));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(t => t.Status == Status);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _technicianRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Technician).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(t => EF.Property<object>(t, sortBy))
                    : query.OrderByDescending(t => EF.Property<object>(t, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var technicians = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (technicians, paginationMetadata);
        }

        public async Task<Technician> GetTechnicianByIdAsync(int id)
        {
            return await _technicianRepository.Query()
                .Where(t => t.TechnicianId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Technician> CreateTechnicianAsync(Technician technician)
        {
            await _technicianRepository.AddAsync(technician);
            await _technicianRepository.SaveChangesAsync();
            return technician;
        }

        public async Task<Technician> UpdateTechnicianAsync(int id, Technician technician)
        {
            var existingTechnician = await _technicianRepository.GetByIdAsync(id);
            if (existingTechnician == null)
            {
                return null;
            }

            existingTechnician.FirstName = technician.FirstName;
            existingTechnician.LastName = technician.LastName;
            existingTechnician.PhoneNumber = technician.PhoneNumber;
            existingTechnician.Email = technician.Email;
            existingTechnician.Status = technician.Status;

            await _technicianRepository.UpdateAsync(existingTechnician);
            await _technicianRepository.SaveChangesAsync();

            return existingTechnician;
        }

        public async Task<bool> DeleteTechnicianAsync(int id)
        {
            var technician = await _technicianRepository.GetByIdAsync(id);
            if (technician == null)
            {
                return false;
            }

            await _technicianRepository.DeleteAsync(id);
            await _technicianRepository.SaveChangesAsync();

            return true;
        }
    }
}