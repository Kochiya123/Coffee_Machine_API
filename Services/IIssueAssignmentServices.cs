using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IIssueAssignmentService
    {
        Task<(IEnumerable<IssueAssignment> IssueAssignments, PaginationMetadata Pagination)> GetIssueAssignmentsAsync(
            int? AssignmentId, DateTime? AssignedDate, int? Status, int? IssueId, int? TechnicianId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<IssueAssignment> GetIssueAssignmentByIdAsync(int id);
        Task<IssueAssignment> CreateIssueAssignmentAsync(IssueAssignment issueAssignment);
        Task<IssueAssignment> UpdateIssueAssignmentAsync(int id, IssueAssignment issueAssignment);
        Task<bool> DeleteIssueAssignmentAsync(int id);
    }

    public class IssueAssignmentService : IIssueAssignmentService
    {
        private readonly IRepository<IssueAssignment> _issueAssignmentRepository;

        public IssueAssignmentService(IRepository<IssueAssignment> issueAssignmentRepository)
        {
            _issueAssignmentRepository = issueAssignmentRepository;
        }

        public async Task<(IEnumerable<IssueAssignment> IssueAssignments, PaginationMetadata Pagination)> GetIssueAssignmentsAsync(
            int? AssignmentId, DateTime? AssignedDate, int? Status, int? IssueId, int? TechnicianId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _issueAssignmentRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (AssignmentId.HasValue)
            {
                query = query.Where(ia => ia.AssignmentId == AssignmentId);
                hasFilters = true;
            }
            if (AssignedDate.HasValue)
            {
                query = query.Where(ia => ia.AssignedDate == AssignedDate);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(ia => ia.Status == Status);
                hasFilters = true;
            }
            if (IssueId.HasValue)
            {
                query = query.Where(ia => ia.IssueId == IssueId);
                hasFilters = true;
            }
            if (TechnicianId.HasValue)
            {
                query = query.Where(ia => ia.TechnicianId == TechnicianId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _issueAssignmentRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(IssueAssignment).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(ia => EF.Property<object>(ia, sortBy))
                    : query.OrderByDescending(ia => EF.Property<object>(ia, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var issueAssignments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (issueAssignments, paginationMetadata);
        }

        public async Task<IssueAssignment> GetIssueAssignmentByIdAsync(int id)
        {
            return await _issueAssignmentRepository.Query()
                .Where(ia => ia.AssignmentId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IssueAssignment> CreateIssueAssignmentAsync(IssueAssignment issueAssignment)
        {
            await _issueAssignmentRepository.AddAsync(issueAssignment);
            await _issueAssignmentRepository.SaveChangesAsync();
            return issueAssignment;
        }

        public async Task<IssueAssignment> UpdateIssueAssignmentAsync(int id, IssueAssignment issueAssignment)
        {
            var existingIssueAssignment = await _issueAssignmentRepository.GetByIdAsync(id);
            if (existingIssueAssignment == null)
            {
                return null;
            }

            existingIssueAssignment.AssignedDate = issueAssignment.AssignedDate;
            existingIssueAssignment.Status = issueAssignment.Status;
            existingIssueAssignment.IssueId = issueAssignment.IssueId;
            existingIssueAssignment.TechnicianId = issueAssignment.TechnicianId;

            await _issueAssignmentRepository.UpdateAsync(existingIssueAssignment);
            await _issueAssignmentRepository.SaveChangesAsync();

            return existingIssueAssignment;
        }

        public async Task<bool> DeleteIssueAssignmentAsync(int id)
        {
            var issueAssignment = await _issueAssignmentRepository.GetByIdAsync(id);
            if (issueAssignment == null)
            {
                return false;
            }

            await _issueAssignmentRepository.DeleteAsync(id);
            await _issueAssignmentRepository.SaveChangesAsync();

            return true;
        }
    }
}