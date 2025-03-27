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
        Task<(IEnumerable<TechnicianDto> Technicians, PaginationMetadata Pagination)> GetTechniciansAsync(string? firstName, string? lastName, string? phoneNumber, string? email, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<TechnicianDto> GetTechnicianByIdAsync(int id);
        Task<TechnicianDto> CreateTechnicianAsync(TechnicianDto technicianDto);
        Task<TechnicianDto> UpdateTechnicianAsync(int id, TechnicianDto technicianDto);
        Task<bool> DeleteTechnicianAsync(int id);
    }

    public class TechnicianService : ITechnicianService
    {
        private readonly IRepository<Technician> _technicianRepository;

        public TechnicianService(IRepository<Technician> technicianRepository)
        {
            _technicianRepository = technicianRepository;
        }

        public async Task<(IEnumerable<TechnicianDto> Technicians, PaginationMetadata Pagination)> GetTechniciansAsync(string? firstName, string? lastName, string? phoneNumber, string? email, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _technicianRepository.Query();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(t => t.FirstName == firstName);
            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(t => t.LastName == lastName);
            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(t => t.PhoneNumber == phoneNumber);
            if (!string.IsNullOrEmpty(email))
                query = query.Where(t => t.Email == email);
            if (status.HasValue)
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Technician).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(t => EF.Property<object>(t, sortBy))
                    : query.OrderByDescending(t => EF.Property<object>(t, sortBy));
            }

            var totalCount = await query.CountAsync();
            var technicians = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var technicianDtos = technicians.Select(t => new TechnicianDto
            {
                TechnicianId = t.TechnicianId,
                FirstName = t.FirstName,
                LastName = t.LastName,
                PhoneNumber = t.PhoneNumber,
                Email = t.Email,
                Status = t.Status,
                IssueAssignments = t.IssueAssignments,
                IssueResolutions = t.IssueResolutions,
                MachineLogs = t.MachineLogs
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (technicianDtos, paginationMetadata);
        }

        public async Task<TechnicianDto> GetTechnicianByIdAsync(int id)
        {
            var technician = await _technicianRepository.GetByIdAsync(id);
            if (technician == null)
                return null;

            return new TechnicianDto
            {
                TechnicianId = technician.TechnicianId,
                FirstName = technician.FirstName,
                LastName = technician.LastName,
                PhoneNumber = technician.PhoneNumber,
                Email = technician.Email,
                Status = technician.Status,
                IssueAssignments = technician.IssueAssignments,
                IssueResolutions = technician.IssueResolutions,
                MachineLogs = technician.MachineLogs
            };
        }

        public async Task<TechnicianDto> CreateTechnicianAsync(TechnicianDto technicianDto)
        {
            var technician = new Technician
            {
                FirstName = technicianDto.FirstName,
                LastName = technicianDto.LastName,
                PhoneNumber = technicianDto.PhoneNumber,
                Email = technicianDto.Email,
                Status = technicianDto.Status,
                IssueAssignments = technicianDto.IssueAssignments,
                IssueResolutions = technicianDto.IssueResolutions,
                MachineLogs = technicianDto.MachineLogs
            };

            await _technicianRepository.AddAsync(technician);
            await _technicianRepository.SaveChangesAsync();
            technicianDto.TechnicianId = technician.TechnicianId;
            return technicianDto;
        }

        public async Task<TechnicianDto> UpdateTechnicianAsync(int id, TechnicianDto technicianDto)
        {
            var technician = await _technicianRepository.GetByIdAsync(id);
            if (technician == null)
                return null;

            technician.FirstName = technicianDto.FirstName;
            technician.LastName = technicianDto.LastName;
            technician.PhoneNumber = technicianDto.PhoneNumber;
            technician.Email = technicianDto.Email;
            technician.Status = technicianDto.Status;
            technician.IssueAssignments = technicianDto.IssueAssignments;
            technician.IssueResolutions = technicianDto.IssueResolutions;
            technician.MachineLogs = technicianDto.MachineLogs;

            await _technicianRepository.UpdateAsync(technician);
            await _technicianRepository.SaveChangesAsync();

            return technicianDto;
        }

        public async Task<bool> DeleteTechnicianAsync(int id)
        {
            var technician = await _technicianRepository.GetByIdAsync(id);
            if (technician == null)
                return false;

            await _technicianRepository.DeleteAsync(id);
            await _technicianRepository.SaveChangesAsync();
            return true;
        }
    }
}