using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IMachineService
    {
        Task<(IEnumerable<MachineDto> Machines, PaginationMetadata Pagination)> GetMachinesAsync(string? machineCode, string? machineName, DateOnly? installationDate, int? status, long? storeId, int? machineTypeId, string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineDto> GetMachineByIdAsync(int id);
        Task<MachineDto> CreateMachineAsync(MachineDto machineDto);
        Task<MachineDto> UpdateMachineAsync(int id, MachineDto machineDto);
        Task<bool> DeleteMachineAsync(int id);
    }

    public class MachineService : IMachineService
    {
        private readonly IRepository<Machine> _machineRepository;

        public MachineService(IRepository<Machine> machineRepository)
        {
            _machineRepository = machineRepository;
        }

        public async Task<(IEnumerable<MachineDto> Machines, PaginationMetadata Pagination)> GetMachinesAsync(string? machineCode, string? machineName, DateOnly? installationDate, int? status, long? storeId, int? machineTypeId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineRepository.Query();

            if (!string.IsNullOrEmpty(machineCode))
                query = query.Where(m => m.MachineCode == machineCode);
            if (!string.IsNullOrEmpty(machineName))
                query = query.Where(m => m.MachineName == machineName);
            if (installationDate.HasValue)
                query = query.Where(m => m.InstallationDate == installationDate);
            if (status.HasValue)
                query = query.Where(m => m.Status == status);
            if (storeId.HasValue)
                query = query.Where(m => m.StoreId == storeId);
            if (machineTypeId.HasValue)
                query = query.Where(m => m.MachineTypeId == machineTypeId);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Machine).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(m => EF.Property<object>(m, sortBy))
                    : query.OrderByDescending(m => EF.Property<object>(m, sortBy));
            }

            var totalCount = await query.CountAsync();
            var machines = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var machineDtos = machines.Select(m => new MachineDto
            {
                MachineId = m.MachineId,
                MachineCode = m.MachineCode,
                MachineName = m.MachineName,
                InstallationDate = m.InstallationDate,
                Status = m.Status,
                StoreId = m.StoreId,
                MachineTypeId = m.MachineTypeId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (machineDtos, paginationMetadata);
        }

           public async Task<MachineDto> GetMachineByIdAsync(int id)
        {
            
            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
                return null;

            return new MachineDto
            {
                MachineId = machine.MachineId,
                MachineCode = machine.MachineCode,
                MachineName = machine.MachineName,
                InstallationDate = machine.InstallationDate,
                Status = machine.Status,
                StoreId = machine.StoreId,
                MachineTypeId = machine.MachineTypeId
            };
        }

        public async Task<MachineDto> CreateMachineAsync(MachineDto machineDto)
        {
            var machine = new Machine
            {
                MachineCode = machineDto.MachineCode,
                MachineName = machineDto.MachineName,
                InstallationDate = machineDto.InstallationDate,
                Status = machineDto.Status,
                StoreId = machineDto.StoreId,
                MachineTypeId = machineDto.MachineTypeId
            };

            await _machineRepository.AddAsync(machine);
            await _machineRepository.SaveChangesAsync();
            machineDto.MachineId = machine.MachineId;
            return machineDto;
        }

        public async Task<MachineDto> UpdateMachineAsync(int id, MachineDto machineDto)
        {
            
            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
                return null;

            machine.MachineCode = machineDto.MachineCode;
            machine.MachineName = machineDto.MachineName;
            machine.InstallationDate = machineDto.InstallationDate;
            machine.Status = machineDto.Status;
            machine.StoreId = machineDto.StoreId;
            machine.MachineTypeId = machineDto.MachineTypeId;

            await _machineRepository.UpdateAsync(machine);
            await _machineRepository.SaveChangesAsync();

            return machineDto;
        }

        public async Task<bool> DeleteMachineAsync(int id)
        {
            
            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
                return false;

            await _machineRepository.DeleteAsync(id);
            await _machineRepository.SaveChangesAsync();
            return true;
        }
    }
}