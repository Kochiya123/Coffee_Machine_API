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
        Task<(IEnumerable<Machine> Machines, PaginationMetadata Pagination)> GetMachinesAsync(
            int? MachineId, string? MachineName, DateOnly? InstallationDate, int? Status, long? StoreId, int? MachineTypeId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Machine> GetMachineByIdAsync(int id);
        Task<Machine> CreateMachineAsync(Machine machine);
        Task<Machine> UpdateMachineAsync(int id, Machine machine);
        Task<bool> DeleteMachineAsync(int id);
    }

    public class MachineService : IMachineService
    {
        private readonly IRepository<Machine> _machineRepository;

        public MachineService(IRepository<Machine> machineRepository)
        {
            _machineRepository = machineRepository;
        }

        public async Task<(IEnumerable<Machine> Machines, PaginationMetadata Pagination)> GetMachinesAsync(
            int? MachineId, string? MachineName, DateOnly? InstallationDate, int? Status, long? StoreId, int? MachineTypeId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (MachineId.HasValue)
            {
                query = query.Where(m => m.MachineId == MachineId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(MachineName))
            {
                query = query.Where(m => m.MachineName.Contains(MachineName));
                hasFilters = true;
            }
            if (InstallationDate.HasValue)
            {
                query = query.Where(m => m.InstallationDate == InstallationDate);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(m => m.Status == Status);
                hasFilters = true;
            }
            if (StoreId.HasValue)
            {
                query = query.Where(m => m.StoreId == StoreId);
                hasFilters = true;
            }
            if (MachineTypeId.HasValue)
            {
                query = query.Where(m => m.MachineTypeId == MachineTypeId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _machineRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Machine).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(m => EF.Property<object>(m, sortBy))
                    : query.OrderByDescending(m => EF.Property<object>(m, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var machines = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (machines, paginationMetadata);
        }

        public async Task<Machine> GetMachineByIdAsync(int id)
        {
            return await _machineRepository.Query()
                .Where(m => m.MachineId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Machine> CreateMachineAsync(Machine machine)
        {
            await _machineRepository.AddAsync(machine);
            await _machineRepository.SaveChangesAsync();
            return machine;
        }

        public async Task<Machine> UpdateMachineAsync(int id, Machine machine)
        {
            var existingMachine = await _machineRepository.GetByIdAsync(id);
            if (existingMachine == null)
            {
                return null;
            }

            existingMachine.MachineName = machine.MachineName;
            existingMachine.InstallationDate = machine.InstallationDate;
            existingMachine.Status = machine.Status;
            existingMachine.StoreId = machine.StoreId;
            existingMachine.MachineTypeId = machine.MachineTypeId;

            await _machineRepository.UpdateAsync(existingMachine);
            await _machineRepository.SaveChangesAsync();

            return existingMachine;
        }

        public async Task<bool> DeleteMachineAsync(int id)
        {
            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
            {
                return false;
            }

            await _machineRepository.DeleteAsync(id);
            await _machineRepository.SaveChangesAsync();

            return true;
        }
    }
}