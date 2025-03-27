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
        Task<(IEnumerable<PaymentDto> Payments, PaginationMetadata Pagination)> GetPaymentsAsync(string? paymentCode, string? paymentMethod, DateTime? paymentDate, int? paymentStatus, int? status, int? orderId, string sortBy, bool isAscending, int page, int pageSize);
        Task<PaymentDto> GetPaymentByIdAsync(int id);
        Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto);
        Task<PaymentDto> UpdatePaymentAsync(int id, PaymentDto paymentDto);
        Task<bool> DeletePaymentAsync(int id);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;

        public PaymentService(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<(IEnumerable<PaymentDto> Payments, PaginationMetadata Pagination)> GetPaymentsAsync(string? paymentCode, string? paymentMethod, DateTime? paymentDate, int? paymentStatus, int? status, int? orderId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _paymentRepository.Query();

            if (!string.IsNullOrEmpty(paymentCode))
                query = query.Where(p => p.PaymentCode == paymentCode);
            if (!string.IsNullOrEmpty(paymentMethod))
                query = query.Where(p => p.PaymentMethod == paymentMethod);
            if (paymentDate.HasValue)
                query = query.Where(p => p.PaymentDate == paymentDate);
            if (paymentStatus.HasValue)
                query = query.Where(p => p.PaymentStatus == paymentStatus);
            if (status.HasValue)
                query = query.Where(p => p.Status == status);
            if (orderId.HasValue)
                query = query.Where(p => p.OrderId == orderId);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Payment).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                    : query.OrderByDescending(p => EF.Property<object>(p, sortBy));
            }

            var totalCount = await query.CountAsync();
            var payments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var paymentDtos = payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                PaymentCode = p.PaymentCode,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                Status = p.Status,
                OrderId = p.OrderId,
                Coupons = p.Coupons,
                Transactions = p.Transactions
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (paymentDtos, paginationMetadata);
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return null;

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                PaymentCode = payment.PaymentCode,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                PaymentStatus = payment.PaymentStatus,
                Status = payment.Status,
                OrderId = payment.OrderId,
                Coupons = payment.Coupons,
                Transactions = payment.Transactions
            };
        }

        public async Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                PaymentCode = paymentDto.PaymentCode,
                PaymentMethod = paymentDto.PaymentMethod,
                PaymentDate = paymentDto.PaymentDate,
                PaymentStatus = paymentDto.PaymentStatus,
                Status = paymentDto.Status,
                OrderId = paymentDto.OrderId,
                Coupons = paymentDto.Coupons,
                Transactions = paymentDto.Transactions
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            paymentDto.PaymentId = payment.PaymentId;
            return paymentDto;
        }

        public async Task<PaymentDto> UpdatePaymentAsync(int id, PaymentDto paymentDto)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return null;

            payment.PaymentCode = paymentDto.PaymentCode;
            payment.PaymentMethod = paymentDto.PaymentMethod;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.PaymentStatus = paymentDto.PaymentStatus;
            payment.Status = paymentDto.Status;
            payment.OrderId = paymentDto.OrderId;
            payment.Coupons = paymentDto.Coupons;
            payment.Transactions = paymentDto.Transactions;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return paymentDto;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return false;

            await _paymentRepository.DeleteAsync(id);
            await _paymentRepository.SaveChangesAsync();
            return true;
        }
    }
}