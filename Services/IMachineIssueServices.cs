using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IMachineIssueService
    {
        Task<(IEnumerable<MachineIssue> MachineIssues, PaginationMetadata Pagination)> GetMachineIssuesAsync(
            int? IssueId, DateTime? ReportDate, string? IssueDescription, int? Status, int? MachineId, long? ReportedBy,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineIssue> GetMachineIssueByIdAsync(int id);
        Task<MachineIssue> CreateMachineIssueAsync(MachineIssue machineIssue);
        Task<MachineIssue> UpdateMachineIssueAsync(int id, MachineIssue machineIssue);
        Task<bool> DeleteMachineIssueAsync(int id);
    }

    public class MachineIssueService : IMachineIssueService
    {
        private readonly IRepository<MachineIssue> _machineIssueRepository;

        public MachineIssueService(IRepository<MachineIssue> machineIssueRepository)
        {
            _machineIssueRepository = machineIssueRepository;
        }

        public async Task<(IEnumerable<MachineIssue> MachineIssues, PaginationMetadata Pagination)> GetMachineIssuesAsync(
            int? IssueId, DateTime? ReportDate, string? IssueDescription, int? Status, int? MachineId, long? ReportedBy,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineIssueRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (IssueId.HasValue)
            {
                query = query.Where(mi => mi.IssueId == IssueId);
                hasFilters = true;
            }
            if (ReportDate.HasValue)
            {
                query = query.Where(mi => mi.ReportDate == ReportDate);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(IssueDescription))
            {
                query = query.Where(mi => mi.IssueDescription.Contains(IssueDescription));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(mi => mi.Status == Status);
                hasFilters = true;
            }
            if (MachineId.HasValue)
            {
                query = query.Where(mi => mi.MachineId == MachineId);
                hasFilters = true;
            }
            if (ReportedBy.HasValue)
            {
                query = query.Where(mi => mi.ReportedBy == ReportedBy);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _machineIssueRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineIssue).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(mi => EF.Property<object>(mi, sortBy))
                    : query.OrderByDescending(mi => EF.Property<object>(mi, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var machineIssues = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (machineIssues, paginationMetadata);
        }

        public async Task<MachineIssue> GetMachineIssueByIdAsync(int id)
        {
            return await _machineIssueRepository.Query()
                .Where(mi => mi.IssueId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MachineIssue> CreateMachineIssueAsync(MachineIssue machineIssue)
        {
            await _machineIssueRepository.AddAsync(machineIssue);
            await _machineIssueRepository.SaveChangesAsync();
            return machineIssue;
        }

        public async Task<MachineIssue> UpdateMachineIssueAsync(int id, MachineIssue machineIssue)
        {
            var existingMachineIssue = await _machineIssueRepository.GetByIdAsync(id);
            if (existingMachineIssue == null)
            {
                return null;
            }

            existingMachineIssue.ReportDate = machineIssue.ReportDate;
            existingMachineIssue.IssueDescription = machineIssue.IssueDescription;
            existingMachineIssue.Status = machineIssue.Status;
            existingMachineIssue.MachineId = machineIssue.MachineId;
            existingMachineIssue.ReportedBy = machineIssue.ReportedBy;

            await _machineIssueRepository.UpdateAsync(existingMachineIssue);
            await _machineIssueRepository.SaveChangesAsync();

            return existingMachineIssue;
        }

        public async Task<bool> DeleteMachineIssueAsync(int id)
        {
            var machineIssue = await _machineIssueRepository.GetByIdAsync(id);
            if (machineIssue == null)
            {
                return false;
            }

            await _machineIssueRepository.DeleteAsync(id);
            await _machineIssueRepository.SaveChangesAsync();

            return true;
        }
    }
}