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
        Task<(IEnumerable<MachineTypeDto> Types, PaginationMetadata Pagination)> GetMachineTypesAsync(string? typeName, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineTypeDto> GetMachineTypeByIdAsync(int id);
        Task<MachineTypeDto> CreateMachineTypeAsync(MachineTypeDto typeDto);
        Task<MachineTypeDto> UpdateMachineTypeAsync(int id, MachineTypeDto typeDto);
        Task<bool> DeleteMachineTypeAsync(int id);
    }

    public class MachineTypeService : IMachineTypeService
    {
        private readonly IRepository<MachineType> _machineTypeRepository;

        public MachineTypeService(IRepository<MachineType> machineTypeRepository)
        {
            _machineTypeRepository = machineTypeRepository;
        }

        public async Task<(IEnumerable<MachineTypeDto> Types, PaginationMetadata Pagination)> GetMachineTypesAsync(string? typeName, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineTypeRepository.Query();

            if (!string.IsNullOrEmpty(typeName))
                query = query.Where(t => t.TypeName == typeName);
            if (status.HasValue)
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineType).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(t => EF.Property<object>(t, sortBy))
                    : query.OrderByDescending(t => EF.Property<object>(t, sortBy));
            }

            var totalCount = await query.CountAsync();
            var types = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var typeDtos = types.Select(t => new MachineTypeDto
            {
                MachineTypeId = t.MachineTypeId,
                TypeName = t.TypeName,
                MachineDescription = t.MachineDescription,
                Status = t.Status
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (typeDtos, paginationMetadata);
        }

        public async Task<MachineTypeDto> GetMachineTypeByIdAsync(int id)
        {
            
            var type = await _machineTypeRepository.GetByIdAsync(id);
            if (type == null)
                return null;

            return new MachineTypeDto
            {
                MachineTypeId = type.MachineTypeId,
                TypeName = type.TypeName,
                MachineDescription = type.MachineDescription,
                Status = type.Status
            };
        }

        public async Task<MachineTypeDto> CreateMachineTypeAsync(MachineTypeDto typeDto)
        {
            var type = new MachineType
            {
                TypeName = typeDto.TypeName,
                MachineDescription = typeDto.MachineDescription,
                Status = typeDto.Status
            };

            await _machineTypeRepository.AddAsync(type);
            await _machineTypeRepository.SaveChangesAsync();
            typeDto.MachineTypeId = type.MachineTypeId;
            return typeDto;
        }

        public async Task<MachineTypeDto> UpdateMachineTypeAsync(int id, MachineTypeDto typeDto)
        {
            var type = await _machineTypeRepository.GetByIdAsync(id);
            if (type == null)
                return null;

            type.TypeName = typeDto.TypeName;
            type.MachineDescription = typeDto.MachineDescription;
            type.Status = typeDto.Status;

            await _machineTypeRepository.UpdateAsync(type);
            await _machineTypeRepository.SaveChangesAsync();

            return typeDto;
        }

        public async Task<bool> DeleteMachineTypeAsync(int id)
        {
            
            var type = await _machineTypeRepository.GetByIdAsync(id);
            if (type == null)
                return false;

            await _machineTypeRepository.DeleteAsync(id);
            await _machineTypeRepository.SaveChangesAsync();
            return true;
        }
    }
}