using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IMachineLogService
    {
        Task<(IEnumerable<MachineLog> MachineLogs, PaginationMetadata Pagination)> GetMachineLogsAsync(
            int? LogId, DateTime? LogDate, string? LogDescription, int? LogType, int? Status, int? MachineId, int? TechnicianId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineLog> GetMachineLogByIdAsync(int id);
        Task<MachineLog> CreateMachineLogAsync(MachineLog machineLog);
        Task<MachineLog> UpdateMachineLogAsync(int id, MachineLog machineLog);
        Task<bool> DeleteMachineLogAsync(int id);
    }

    public class MachineLogService : IMachineLogService
    {
        private readonly IRepository<MachineLog> _machineLogRepository;

        public MachineLogService(IRepository<MachineLog> machineLogRepository)
        {
            _machineLogRepository = machineLogRepository;
        }

        public async Task<(IEnumerable<MachineLog> MachineLogs, PaginationMetadata Pagination)> GetMachineLogsAsync(
            int? LogId, DateTime? LogDate, string? LogDescription, int? LogType, int? Status, int? MachineId, int? TechnicianId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _machineLogRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (LogId.HasValue)
            {
                query = query.Where(ml => ml.LogId == LogId);
                hasFilters = true;
            }
            if (LogDate.HasValue)
            {
                query = query.Where(ml => ml.LogDate == LogDate);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(LogDescription))
            {
                query = query.Where(ml => ml.LogDescription.Contains(LogDescription));
                hasFilters = true;
            }
            if (LogType.HasValue)
            {
                query = query.Where(ml => ml.LogType == LogType);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(ml => ml.Status == Status);
                hasFilters = true;
            }
            if (MachineId.HasValue)
            {
                query = query.Where(ml => ml.MachineId == MachineId);
                hasFilters = true;
            }
            if (TechnicianId.HasValue)
            {
                query = query.Where(ml => ml.TechnicianId == TechnicianId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _machineLogRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineLog).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(ml => EF.Property<object>(ml, sortBy))
                    : query.OrderByDescending(ml => EF.Property<object>(ml, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var machineLogs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (machineLogs, paginationMetadata);
        }

        public async Task<MachineLog> GetMachineLogByIdAsync(int id)
        {
            return await _machineLogRepository.Query()
                .Where(ml => ml.LogId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MachineLog> CreateMachineLogAsync(MachineLog machineLog)
        {
            await _machineLogRepository.AddAsync(machineLog);
            await _machineLogRepository.SaveChangesAsync();
            return machineLog;
        }

        public async Task<MachineLog> UpdateMachineLogAsync(int id, MachineLog machineLog)
        {
            var existingMachineLog = await _machineLogRepository.GetByIdAsync(id);
            if (existingMachineLog == null)
            {
                return null;
            }

            existingMachineLog.LogDate = machineLog.LogDate;
            existingMachineLog.LogDescription = machineLog.LogDescription;
            existingMachineLog.LogType = machineLog.LogType;
            existingMachineLog.Status = machineLog.Status;
            existingMachineLog.MachineId = machineLog.MachineId;
            existingMachineLog.TechnicianId = machineLog.TechnicianId;

            await _machineLogRepository.UpdateAsync(existingMachineLog);
            await _machineLogRepository.SaveChangesAsync();

            return existingMachineLog;
        }

        public async Task<bool> DeleteMachineLogAsync(int id)
        {
            var machineLog = await _machineLogRepository.GetByIdAsync(id);
            if (machineLog == null)
            {
                return false;
            }

            await _machineLogRepository.DeleteAsync(id);
            await _machineLogRepository.SaveChangesAsync();

            return true;
        }
    }
}