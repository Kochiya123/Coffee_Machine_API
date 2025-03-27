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
        Task<(IEnumerable<MachineLogDto> Logs, PaginationMetadata Pagination)> GetMachineLogsAsync(int? machineId, int? technicianId, long? performedBy, int? logType, int? status, DateTime? logDate, string sortBy, bool isAscending, int page, int pageSize);
        Task<MachineLogDto> GetMachineLogByIdAsync(int id);
        Task<MachineLogDto> CreateMachineLogAsync(MachineLogDto logDto);
        Task<MachineLogDto> UpdateMachineLogAsync(int id, MachineLogDto logDto);
        Task<bool> DeleteMachineLogAsync(int id);
    }

    public class MachineLogService : IMachineLogService
    {
        private readonly IRepository<MachineLog> _logRepository;

        public MachineLogService(IRepository<MachineLog> logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<(IEnumerable<MachineLogDto> Logs, PaginationMetadata Pagination)> GetMachineLogsAsync(int? machineId, int? technicianId, long? performedBy, int? logType, int? status, DateTime? logDate, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _logRepository.Query();

            if (machineId.HasValue)
                query = query.Where(l => l.MachineId == machineId);
            if (technicianId.HasValue)
                query = query.Where(l => l.TechnicianId == technicianId);
            if (performedBy.HasValue)
                query = query.Where(l => l.PerformedBy == performedBy);
            if (logType.HasValue)
                query = query.Where(l => l.LogType == logType);
            if (status.HasValue)
                query = query.Where(l => l.Status == status);
            if (logDate.HasValue)
                query = query.Where(l => l.LogDate == logDate);

            if (!string.IsNullOrEmpty(sortBy) && typeof(MachineLog).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(l => EF.Property<object>(l, sortBy))
                    : query.OrderByDescending(l => EF.Property<object>(l, sortBy));
            }

            var totalCount = await query.CountAsync();
            var logs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var logDtos = logs.Select(l => new MachineLogDto
            {
                LogId = l.LogId,
                LogDate = l.LogDate,
                LogDescription = l.LogDescription,
                LogType = l.LogType,
                PerformedBy = l.PerformedBy,
                Status = l.Status,
                MachineId = l.MachineId,
                TechnicianId = l.TechnicianId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (logDtos, paginationMetadata);
        }

        public async Task<MachineLogDto> GetMachineLogByIdAsync(int id)
        {
            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
                return null;

            return new MachineLogDto
            {
                LogId = log.LogId,
                LogDate = log.LogDate,
                LogDescription = log.LogDescription,
                LogType = log.LogType,
                PerformedBy = log.PerformedBy,
                Status = log.Status,
                MachineId = log.MachineId,
                TechnicianId = log.TechnicianId
            };
        }

        public async Task<MachineLogDto> UpdateMachineLogAsync(int id, MachineLogDto logDto)
        {
            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
                return null;

            log.LogDate = logDto.LogDate;
            log.LogDescription = logDto.LogDescription;
            log.LogType = logDto.LogType;
            log.PerformedBy = logDto.PerformedBy;
            log.Status = logDto.Status;
            log.MachineId = logDto.MachineId;
            log.TechnicianId = logDto.TechnicianId;

            await _logRepository.UpdateAsync(log);
            await _logRepository.SaveChangesAsync();

            return logDto;
        }

        public async Task<MachineLogDto> CreateMachineLogAsync(MachineLogDto logDto)
        {
            var log = new MachineLog
            {
                LogDate = logDto.LogDate,
                LogDescription = logDto.LogDescription,
                LogType = logDto.LogType,
                PerformedBy = logDto.PerformedBy,
                Status = logDto.Status,
                MachineId = logDto.MachineId,
                TechnicianId = logDto.TechnicianId
            };

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
            logDto.LogId = log.LogId;
            return logDto;
        }

        public async Task<bool> DeleteMachineLogAsync(int id)
        {
            
            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
                return false;

            await _logRepository.DeleteAsync(id);
            await _logRepository.SaveChangesAsync();
            return true;
        }
    }
}