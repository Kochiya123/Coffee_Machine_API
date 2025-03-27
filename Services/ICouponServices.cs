using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface ICouponService
    {
        Task<(IEnumerable<CouponDto> Coupons, PaginationMetadata Pagination)> GetCouponsAsync(string? couponCode, decimal? minDiscount, decimal? maxDiscount, DateTime? startDate, DateTime? expirationDate, int? status, int? paymentId, int? productId, string sortBy, bool isAscending, int page, int pageSize);
        Task<CouponDto> GetCouponByIdAsync(int id);
        Task<CouponDto> CreateCouponAsync(CouponDto couponDto);
        Task<CouponDto> UpdateCouponAsync(int id, CouponDto couponDto);
        Task<bool> DeleteCouponAsync(int id);
    }

    public class CouponService : ICouponService
    {
        private readonly IRepository<Coupon> _couponRepository;

        public CouponService(IRepository<Coupon> couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<(IEnumerable<CouponDto> Coupons, PaginationMetadata Pagination)> GetCouponsAsync(string? couponCode, decimal? minDiscount, decimal? maxDiscount, DateTime? startDate, DateTime? expirationDate, int? status, int? paymentId, int? productId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _couponRepository.Query();

            if (!string.IsNullOrEmpty(couponCode))
                query = query.Where(c => c.CouponCode.Contains(couponCode));
            if (minDiscount.HasValue)
                query = query.Where(c => c.DiscountAmount >= minDiscount);
            if (maxDiscount.HasValue)
                query = query.Where(c => c.DiscountAmount <= maxDiscount);
            if (startDate.HasValue)
                query = query.Where(c => c.StartDate >= startDate);
            if (expirationDate.HasValue)
                query = query.Where(c => c.ExpirationDate <= expirationDate);
            if (status.HasValue)
                query = query.Where(c => c.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Coupon).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(c => EF.Property<object>(c, sortBy))
                    : query.OrderByDescending(c => EF.Property<object>(c, sortBy));
            }

            var totalCount = await query.CountAsync();
            var coupons = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var couponDtos = coupons.Select(c => new CouponDto
            {
                CouponId = c.CouponId,
                CouponCode = c.CouponCode,
                DiscountAmount = c.DiscountAmount,
                StartDate = c.StartDate,
                ExpirationDate = c.ExpirationDate,
                AmountItem = c.AmountItem,
                Status = c.Status,
                PaymentId = c.PaymentId,
                ProductId = c.ProductId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (couponDtos, paginationMetadata);
        }

        public async Task<CouponDto> GetCouponByIdAsync(int id)
        {
            
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
                return null;

            return new CouponDto
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                DiscountAmount = coupon.DiscountAmount,
                StartDate = coupon.StartDate,
                ExpirationDate = coupon.ExpirationDate,
                AmountItem = coupon.AmountItem,
                Status = coupon.Status,
                PaymentId = coupon.PaymentId,
                ProductId = coupon.ProductId
            };
        }

        public async Task<CouponDto> CreateCouponAsync(CouponDto couponDto)
        {
            var coupon = new Coupon
            {
                CouponCode = couponDto.CouponCode,
                DiscountAmount = couponDto.DiscountAmount,
                StartDate = couponDto.StartDate,
                ExpirationDate = couponDto.ExpirationDate,
                AmountItem = couponDto.AmountItem,
                Status = couponDto.Status,
                PaymentId = couponDto.PaymentId,
                ProductId = couponDto.ProductId
            };

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();
            couponDto.CouponId = coupon.CouponId;
            return couponDto;
        }

        public async Task<CouponDto> UpdateCouponAsync(int id, CouponDto couponDto)
        {
            
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
                return null;

            coupon.CouponCode = couponDto.CouponCode;
            coupon.DiscountAmount = couponDto.DiscountAmount;
            coupon.StartDate = couponDto.StartDate;
            coupon.ExpirationDate = couponDto.ExpirationDate;
            coupon.AmountItem = couponDto.AmountItem;
            coupon.Status = couponDto.Status;
            coupon.PaymentId = couponDto.PaymentId;
            coupon.ProductId = couponDto.ProductId;

            await _couponRepository.UpdateAsync(coupon);
            await _couponRepository.SaveChangesAsync();

            return couponDto;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
                return false;

            await _couponRepository.DeleteAsync(id);
            await _couponRepository.SaveChangesAsync();
            return true;
        }
    }
}
