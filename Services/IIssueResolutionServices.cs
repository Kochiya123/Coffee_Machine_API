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
        Task<(IEnumerable<IssueResolutionDto> Resolutions, PaginationMetadata Pagination)> GetIssueResolutionsAsync(int? issueId, int? technicianId, int? status, DateTime? resolutionDate, string sortBy, bool isAscending, int page, int pageSize);
        Task<IssueResolutionDto> GetIssueResolutionByIdAsync(int id);
        Task<IssueResolutionDto> CreateIssueResolutionAsync(IssueResolutionDto resolutionDto);
        Task<IssueResolutionDto> UpdateIssueResolutionAsync(int id, IssueResolutionDto resolutionDto);
        Task<bool> DeleteIssueResolutionAsync(int id);
    }

    public class IssueResolutionService : IIssueResolutionService
    {
        private readonly IRepository<IssueResolution> _resolutionRepository;

        public IssueResolutionService(IRepository<IssueResolution> resolutionRepository)
        {
            _resolutionRepository = resolutionRepository;
        }

        public async Task<(IEnumerable<IssueResolutionDto> Resolutions, PaginationMetadata Pagination)> GetIssueResolutionsAsync(int? issueId, int? technicianId, int? status, DateTime? resolutionDate, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _resolutionRepository.Query();

            if (issueId.HasValue)
                query = query.Where(r => r.IssueId == issueId);
            if (technicianId.HasValue)
                query = query.Where(r => r.TechnicianId == technicianId);
            if (status.HasValue)
                query = query.Where(r => r.Status == status);
            if (resolutionDate.HasValue)
                query = query.Where(r => r.ResolutionDate == resolutionDate);

            if (!string.IsNullOrEmpty(sortBy) && typeof(IssueResolution).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(r => EF.Property<object>(r, sortBy))
                    : query.OrderByDescending(r => EF.Property<object>(r, sortBy));
            }

            var totalCount = await query.CountAsync();
            var resolutions = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var resolutionDtos = resolutions.Select(r => new IssueResolutionDto
            {
                ResolutionId = r.ResolutionId,
                ResolutionDate = r.ResolutionDate,
                ResolutionDescription = r.ResolutionDescription,
                Status = r.Status,
                IssueId = r.IssueId,
                TechnicianId = r.TechnicianId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (resolutionDtos, paginationMetadata);
        }

    public async Task<IssueResolutionDto> GetIssueResolutionByIdAsync(int id)
        {
            
            var resolution = await _resolutionRepository.GetByIdAsync(id);
            if (resolution == null)
                return null;

            return new IssueResolutionDto
            {
                ResolutionId = resolution.ResolutionId,
                ResolutionDate = resolution.ResolutionDate,
                ResolutionDescription = resolution.ResolutionDescription,
                Status = resolution.Status,
                IssueId = resolution.IssueId,
                TechnicianId = resolution.TechnicianId
            };
        }

        public async Task<IssueResolutionDto> CreateIssueResolutionAsync(IssueResolutionDto resolutionDto)
        {
            var resolution = new IssueResolution
            {
                ResolutionDate = resolutionDto.ResolutionDate,
                ResolutionDescription = resolutionDto.ResolutionDescription,
                Status = resolutionDto.Status,
                IssueId = resolutionDto.IssueId,
                TechnicianId = resolutionDto.TechnicianId
            };

            await _resolutionRepository.AddAsync(resolution);
            await _resolutionRepository.SaveChangesAsync();
            resolutionDto.ResolutionId = resolution.ResolutionId;
            return resolutionDto;
        }

        public async Task<IssueResolutionDto> UpdateIssueResolutionAsync(int id, IssueResolutionDto resolutionDto)
        {
            long intid = (long)id;
            var resolution = await _resolutionRepository.GetByIdAsync(id);
            if (resolution == null)
                return null;

            resolution.ResolutionDate = resolutionDto.ResolutionDate;
            resolution.ResolutionDescription = resolutionDto.ResolutionDescription;
            resolution.Status = resolutionDto.Status;
            resolution.IssueId = resolutionDto.IssueId;
            resolution.TechnicianId = resolutionDto.TechnicianId;

            await _resolutionRepository.UpdateAsync(resolution);
            await _resolutionRepository.SaveChangesAsync();

            return resolutionDto;
        }

        public async Task<bool> DeleteIssueResolutionAsync(int id)
        {
            
            var resolution = await _resolutionRepository.GetByIdAsync(id);
            if (resolution == null)
                return false;

            await _resolutionRepository.DeleteAsync(id);
            await _resolutionRepository.SaveChangesAsync();
            return true;
        }
    }
}