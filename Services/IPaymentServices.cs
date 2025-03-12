using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IPaymentService
    {
        Task<(IEnumerable<Payment> Payments, PaginationMetadata Pagination)> GetPaymentsAsync(
            int? PaymentId, string? PaymentMethod, DateTime? PaymentDate, int? PaymentStatus, int? Status, int? OrderId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Payment> GetPaymentByIdAsync(int id);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(int id, Payment payment);
        Task<bool> DeletePaymentAsync(int id);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;

        public PaymentService(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<(IEnumerable<Payment> Payments, PaginationMetadata Pagination)> GetPaymentsAsync(
            int? PaymentId, string? PaymentMethod, DateTime? PaymentDate, int? PaymentStatus, int? Status, int? OrderId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _paymentRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (PaymentId.HasValue)
            {
                query = query.Where(p => p.PaymentId == PaymentId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(PaymentMethod))
            {
                query = query.Where(p => p.PaymentMethod.Contains(PaymentMethod));
                hasFilters = true;
            }
            if (PaymentDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate == PaymentDate);
                hasFilters = true;
            }
            if (PaymentStatus.HasValue)
            {
                query = query.Where(p => p.PaymentStatus == PaymentStatus);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(p => p.Status == Status);
                hasFilters = true;
            }
            if (OrderId.HasValue)
            {
                query = query.Where(p => p.OrderId == OrderId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _paymentRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Payment).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                    : query.OrderByDescending(p => EF.Property<object>(p, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var payments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (payments, paginationMetadata);
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepository.Query()
                .Where(p => p.PaymentId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(int id, Payment payment)
        {
            var existingPayment = await _paymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return null;
            }

            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.PaymentDate = payment.PaymentDate;
            existingPayment.PaymentStatus = payment.PaymentStatus;
            existingPayment.Status = payment.Status;
            existingPayment.OrderId = payment.OrderId;

            await _paymentRepository.UpdateAsync(existingPayment);
            await _paymentRepository.SaveChangesAsync();

            return existingPayment;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return false;
            }

            await _paymentRepository.DeleteAsync(id);
            await _paymentRepository.SaveChangesAsync();

            return true;
        }
    }
}