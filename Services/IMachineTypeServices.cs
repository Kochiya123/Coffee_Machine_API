using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IMachineTypeService
    {
        Task<(IEnumerable<MachineType> MachineTypes, PaginationMetadata Pagination)> GetMachineTypesAsync(
            int? MachineTypeId, string? TypeName, int? Status,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineType> GetMachineTypeByIdAsync(int id);
        Task<MachineType> CreateMachineTypeAsync(MachineType machineType);
        Task<MachineType> UpdateMachineTypeAsync(int id, MachineType machineType);
        Task<bool> DeleteMachineTypeAsync(int id);
    }

    public class MachineTypeService : IMachineTypeService
    {
        private readonly IRepository<MachineType> _machineTypeRepository;

        public MachineTypeService(IRepository<MachineType> machineTypeRepository)
        {
            _machineTypeRepository = machineTypeRepository;
        }

        public async Task<(IEnumerable<MachineType> MachineTypes, PaginationMetadata Pagination)> GetMachineTypesAsync(
            int? MachineTypeId, string? TypeName, int? Status,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineTypeRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (MachineTypeId.HasValue)
            {
                query = query.Where(mt => mt.MachineTypeId == MachineTypeId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(TypeName))
            {
                query = query.Where(mt => mt.TypeName.Contains(TypeName));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(mt => mt.Status == Status);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _machineTypeRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineType).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(mt => EF.Property<object>(mt, sortBy))
                    : query.OrderByDescending(mt => EF.Property<object>(mt, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var machineTypes = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (machineTypes, paginationMetadata);
        }

        public async Task<MachineType> GetMachineTypeByIdAsync(int id)
        {
            return await _machineTypeRepository.Query()
                .Where(mt => mt.MachineTypeId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MachineType> CreateMachineTypeAsync(MachineType machineType)
        {
            await _machineTypeRepository.AddAsync(machineType);
            await _machineTypeRepository.SaveChangesAsync();
            return machineType;
        }

        public async Task<MachineType> UpdateMachineTypeAsync(int id, MachineType machineType)
        {
            var existingMachineType = await _machineTypeRepository.GetByIdAsync(id);
            if (existingMachineType == null)
            {
                return null;
            }

            existingMachineType.TypeName = machineType.TypeName;
            existingMachineType.MachineDescription = machineType.MachineDescription;
            existingMachineType.Status = machineType.Status;

            await _machineTypeRepository.UpdateAsync(existingMachineType);
            await _machineTypeRepository.SaveChangesAsync();

            return existingMachineType;
        }

        public async Task<bool> DeleteMachineTypeAsync(int id)
        {
            var machineType = await _machineTypeRepository.GetByIdAsync(id);
            if (machineType == null)
            {
                return false;
            }

            await _machineTypeRepository.DeleteAsync(id);
            await _machineTypeRepository.SaveChangesAsync();

            return true;
        }
    }
}