using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IAreaService
    {
        Task<(IEnumerable<Area> Areas, PaginationMetadata Pagination)> GetAreasAsync(string? areaName, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<Area> GetAreaByIdAsync(int id);
        Task<Area> CreateAreaAsync(Area area);
        Task<Area> UpdateAreaAsync(int id, Area area);
        Task<bool> DeleteAreaAsync(int id);
    }

    public class AreaService : IAreaService
    {
        private readonly IRepository<Area> _areaRepository;

        public AreaService(IRepository<Area> areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public async Task<(IEnumerable<Area> Areas, PaginationMetadata Pagination)> GetAreasAsync(string? areaName, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _areaRepository.Query();

            if (!string.IsNullOrEmpty(areaName))
                query = query.Where(a => a.AreaName.Contains(areaName));
            if (status.HasValue)
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Area).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(a => EF.Property<object>(a, sortBy))
                    : query.OrderByDescending(a => EF.Property<object>(a, sortBy));
            }

            var totalCount = await query.CountAsync();
            var areas = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (areas, paginationMetadata);
        }

        public async Task<Area> GetAreaByIdAsync(int id)
        {
            return await _areaRepository.GetByIdAsync(id);
        }

        public async Task<Area> CreateAreaAsync(Area area)
        {
            await _areaRepository.AddAsync(area);
            await _areaRepository.SaveChangesAsync();
            return area;
        }

        public async Task<Area> UpdateAreaAsync(int id, Area area)
        {
            var existingArea = await _areaRepository.GetByIdAsync(id);
            if (existingArea == null)
                return null;

            existingArea.AreaName = area.AreaName;
            existingArea.Status = area.Status;

            await _areaRepository.UpdateAsync(existingArea);
            await _areaRepository.SaveChangesAsync();
            return existingArea;
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            var area = await _areaRepository.GetByIdAsync(id);
            if (area == null)
                return false;

            await _areaRepository.DeleteAsync(id);
            await _areaRepository.SaveChangesAsync();
            return true;
        }
    }
}