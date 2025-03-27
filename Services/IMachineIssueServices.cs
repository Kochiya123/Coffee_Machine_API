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
        Task<(IEnumerable<MachineIssueDto> Issues, PaginationMetadata Pagination)> GetMachineIssuesAsync(int? machineId, long? reportedBy, int? status, DateTime? reportDate, string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineIssueDto> GetMachineIssueByIdAsync(int id);
        Task<MachineIssueDto> CreateMachineIssueAsync(MachineIssueDto issueDto);
        Task<MachineIssueDto> UpdateMachineIssueAsync(int id, MachineIssueDto issueDto);
        Task<bool> DeleteMachineIssueAsync(int id);
    }

    public class MachineIssueService : IMachineIssueService
    {
        private readonly IRepository<MachineIssue> _issueRepository;

        public MachineIssueService(IRepository<MachineIssue> issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public async Task<(IEnumerable<MachineIssueDto> Issues, PaginationMetadata Pagination)> GetMachineIssuesAsync(int? machineId, long? reportedBy, int? status, DateTime? reportDate, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _issueRepository.Query();

            if (machineId.HasValue)
                query = query.Where(i => i.MachineId == machineId);
            if (reportedBy.HasValue)
                query = query.Where(i => i.ReportedBy == reportedBy);
            if (status.HasValue)
                query = query.Where(i => i.Status == status);
            if (reportDate.HasValue)
                query = query.Where(i => i.ReportDate == reportDate);

            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineIssue).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(i => EF.Property<object>(i, sortBy))
                    : query.OrderByDescending(i => EF.Property<object>(i, sortBy));
            }

            var totalCount = await query.CountAsync();
            var issues = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var issueDtos = issues.Select(i => new MachineIssueDto
            {
                IssueId = i.IssueId,
                ReportDate = i.ReportDate,
                IssueDescription = i.IssueDescription,
                Status = i.Status,
                MachineId = i.MachineId,
                ReportedBy = i.ReportedBy,
                IssueAssignments = i.IssueAssignments,
                IssueResolutions = i.IssueResolutions
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (issueDtos, paginationMetadata);
        }

        public async Task<MachineIssueDto> GetMachineIssueByIdAsync(int id)
        {
            
            var issue = await _issueRepository.GetByIdAsync(id);
            if (issue == null)
                return null;

            return new MachineIssueDto
            {
                IssueId = issue.IssueId,
                ReportDate = issue.ReportDate,
                IssueDescription = issue.IssueDescription,
                Status = issue.Status,
                MachineId = issue.MachineId,
                ReportedBy = issue.ReportedBy,
                IssueAssignments = issue.IssueAssignments,
                IssueResolutions = issue.IssueResolutions
            };
        }

        public async Task<MachineIssueDto> CreateMachineIssueAsync(MachineIssueDto issueDto)
        {
            var issue = new MachineIssue
            {
                ReportDate = issueDto.ReportDate,
                IssueDescription = issueDto.IssueDescription,
                Status = issueDto.Status,
                MachineId = issueDto.MachineId,
                ReportedBy = issueDto.ReportedBy
            };

            await _issueRepository.AddAsync(issue);
            await _issueRepository.SaveChangesAsync();
            issueDto.IssueId = issue.IssueId;
            return issueDto;
        }

        public async Task<MachineIssueDto> UpdateMachineIssueAsync(int id, MachineIssueDto issueDto)
        {
            
            var issue = await _issueRepository.GetByIdAsync(id);
            if (issue == null)
                return null;

            issue.ReportDate = issueDto.ReportDate;
            issue.IssueDescription = issueDto.IssueDescription;
            issue.Status = issueDto.Status;
            issue.MachineId = issueDto.MachineId;
            issue.ReportedBy = issueDto.ReportedBy;

            await _issueRepository.UpdateAsync(issue);
            await _issueRepository.SaveChangesAsync();

            return issueDto;
        }

        public async Task<bool> DeleteMachineIssueAsync(int id)
        {
            
            var issue = await _issueRepository.GetByIdAsync(id);
            if (issue == null)
                return false;

            await _issueRepository.DeleteAsync(id);
            await _issueRepository.SaveChangesAsync();
            return true;
        }
    }
}