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
        Task<(IEnumerable<IssueAssignmentDto> Assignments, PaginationMetadata Pagination)> GetIssueAssignmentsAsync(int? issueId, int? technicianId, int? status, DateTime? assignedDate, string sortBy, bool isAscending, int page, int pageSize);
        Task<IssueAssignmentDto> GetIssueAssignmentByIdAsync(int id);
        Task<IssueAssignmentDto> CreateIssueAssignmentAsync(IssueAssignmentDto assignmentDto);
        Task<IssueAssignmentDto> UpdateIssueAssignmentAsync(int id, IssueAssignmentDto assignmentDto);
        Task<bool> DeleteIssueAssignmentAsync(int id);
    }

    public class IssueAssignmentService : IIssueAssignmentService
    {
        private readonly IRepository<IssueAssignment> _assignmentRepository;

        public IssueAssignmentService(IRepository<IssueAssignment> assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<(IEnumerable<IssueAssignmentDto> Assignments, PaginationMetadata Pagination)> GetIssueAssignmentsAsync(int? issueId, int? technicianId, int? status, DateTime? assignedDate, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _assignmentRepository.Query();

            if (issueId.HasValue)
                query = query.Where(a => a.IssueId == issueId);
            if (technicianId.HasValue)
                query = query.Where(a => a.TechnicianId == technicianId);
            if (status.HasValue)
                query = query.Where(a => a.Status == status);
            if (assignedDate.HasValue)
                query = query.Where(a => a.AssignedDate == assignedDate);

            if (!string.IsNullOrEmpty(sortBy) && typeof(IssueAssignment).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(a => EF.Property<object>(a, sortBy))
                    : query.OrderByDescending(a => EF.Property<object>(a, sortBy));
            }

            var totalCount = await query.CountAsync();
            var assignments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var assignmentDtos = assignments.Select(a => new IssueAssignmentDto
            {
                AssignmentId = a.AssignmentId,
                AssignedDate = a.AssignedDate,
                Status = a.Status,
                IssueId = a.IssueId,
                TechnicianId = a.TechnicianId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (assignmentDtos, paginationMetadata);
        }

        public async Task<IssueAssignmentDto> GetIssueAssignmentByIdAsync(int id)
        {
            
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return null;

            return new IssueAssignmentDto
            {
                AssignmentId = assignment.AssignmentId,
                AssignedDate = assignment.AssignedDate,
                Status = assignment.Status,
                IssueId = assignment.IssueId,
                TechnicianId = assignment.TechnicianId
            };
        }

        public async Task<IssueAssignmentDto> CreateIssueAssignmentAsync(IssueAssignmentDto assignmentDto)
        {
            var assignment = new IssueAssignment
            {
                AssignedDate = assignmentDto.AssignedDate,
                Status = assignmentDto.Status,
                IssueId = assignmentDto.IssueId,
                TechnicianId = assignmentDto.TechnicianId
            };

            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();
            assignmentDto.AssignmentId = assignment.AssignmentId;
            return assignmentDto;
        }

        public async Task<IssueAssignmentDto> UpdateIssueAssignmentAsync(int id, IssueAssignmentDto assignmentDto)
        {
            
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return null;

            assignment.AssignedDate = assignmentDto.AssignedDate;
            assignment.Status = assignmentDto.Status;
            assignment.IssueId = assignmentDto.IssueId;
            assignment.TechnicianId = assignmentDto.TechnicianId;

            await _assignmentRepository.UpdateAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();

            return assignmentDto;
        }

        public async Task<bool> DeleteIssueAssignmentAsync(int id)
        {
            
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return false;

            await _assignmentRepository.DeleteAsync(id);
            await _assignmentRepository.SaveChangesAsync();
            return true;
        }
    }
}
