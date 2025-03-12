using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IIssueResolutionService
    {
        Task<(IEnumerable<IssueResolution> IssueResolutions, PaginationMetadata Pagination)> GetIssueResolutionsAsync(
            int? ResolutionId, DateTime? ResolutionDate, string? ResolutionDescription, int? Status, int? IssueId, int? TechnicianId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<IssueResolution> GetIssueResolutionByIdAsync(int id);
        Task<IssueResolution> CreateIssueResolutionAsync(IssueResolution issueResolution);
        Task<IssueResolution> UpdateIssueResolutionAsync(int id, IssueResolution issueResolution);
        Task<bool> DeleteIssueResolutionAsync(int id);
    }

    public class IssueResolutionService : IIssueResolutionService
    {
        private readonly IRepository<IssueResolution> _issueResolutionRepository;

        public IssueResolutionService(IRepository<IssueResolution> issueResolutionRepository)
        {
            _issueResolutionRepository = issueResolutionRepository;
        }

        public async Task<(IEnumerable<IssueResolution> IssueResolutions, PaginationMetadata Pagination)> GetIssueResolutionsAsync(
            int? ResolutionId, DateTime? ResolutionDate, string? ResolutionDescription, int? Status, int? IssueId, int? TechnicianId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _issueResolutionRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (ResolutionId.HasValue)
            {
                query = query.Where(ir => ir.ResolutionId == ResolutionId);
                hasFilters = true;
            }
            if (ResolutionDate.HasValue)
            {
                query = query.Where(ir => ir.ResolutionDate == ResolutionDate);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(ResolutionDescription))
            {
                query = query.Where(ir => ir.ResolutionDescription.Contains(ResolutionDescription));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(ir => ir.Status == Status);
                hasFilters = true;
            }
            if (IssueId.HasValue)
            {
                query = query.Where(ir => ir.IssueId == IssueId);
                hasFilters = true;
            }
            if (TechnicianId.HasValue)
            {
                query = query.Where(ir => ir.TechnicianId == TechnicianId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _issueResolutionRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(IssueResolution).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(ir => EF.Property<object>(ir, sortBy))
                    : query.OrderByDescending(ir => EF.Property<object>(ir, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var issueResolutions = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (issueResolutions, paginationMetadata);
        }

        public async Task<IssueResolution> GetIssueResolutionByIdAsync(int id)
        {
            return await _issueResolutionRepository.Query()
                .Where(ir => ir.ResolutionId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IssueResolution> CreateIssueResolutionAsync(IssueResolution issueResolution)
        {
            await _issueResolutionRepository.AddAsync(issueResolution);
            await _issueResolutionRepository.SaveChangesAsync();
            return issueResolution;
        }

        public async Task<IssueResolution> UpdateIssueResolutionAsync(int id, IssueResolution issueResolution)
        {
            var existingIssueResolution = await _issueResolutionRepository.GetByIdAsync(id);
            if (existingIssueResolution == null)
            {
                return null;
            }

            existingIssueResolution.ResolutionDate = issueResolution.ResolutionDate;
            existingIssueResolution.ResolutionDescription = issueResolution.ResolutionDescription;
            existingIssueResolution.Status = issueResolution.Status;
            existingIssueResolution.IssueId = issueResolution.IssueId;
            existingIssueResolution.TechnicianId = issueResolution.TechnicianId;

            await _issueResolutionRepository.UpdateAsync(existingIssueResolution);
            await _issueResolutionRepository.SaveChangesAsync();

            return existingIssueResolution;
        }

        public async Task<bool> DeleteIssueResolutionAsync(int id)
        {
            var issueResolution = await _issueResolutionRepository.GetByIdAsync(id);
            if (issueResolution == null)
            {
                return false;
            }

            await _issueResolutionRepository.DeleteAsync(id);
            await _issueResolutionRepository.SaveChangesAsync();

            return true;
        }
    }
}