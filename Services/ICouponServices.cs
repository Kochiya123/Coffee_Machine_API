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
        Task<(IEnumerable<Coupon> Coupons, PaginationMetadata Pagination)> GetCouponsAsync(
            int? CouponId, string? CouponCode, decimal? DiscountAmount, DateTime? StartDate, DateTime? ExpirationDate, int? Status,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Coupon> GetCouponByIdAsync(int id);
        Task<Coupon> CreateCouponAsync(Coupon coupon);
        Task<Coupon> UpdateCouponAsync(int id, Coupon coupon);
        Task<bool> DeleteCouponAsync(int id);
    }

    public class CouponService : ICouponService
    {
        private readonly IRepository<Coupon> _couponRepository;

        public CouponService(IRepository<Coupon> couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<(IEnumerable<Coupon> Coupons, PaginationMetadata Pagination)> GetCouponsAsync(
            int? CouponId, string? CouponCode, decimal? DiscountAmount, DateTime? StartDate, DateTime? ExpirationDate, int? Status,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _couponRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (CouponId.HasValue)
            {
                query = query.Where(c => c.CouponId == CouponId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(CouponCode))
            {
                query = query.Where(c => c.CouponCode.Contains(CouponCode));
                hasFilters = true;
            }
            if (DiscountAmount.HasValue)
            {
                query = query.Where(c => c.DiscountAmount == DiscountAmount);
                hasFilters = true;
            }
            if (StartDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= StartDate);
                hasFilters = true;
            }
            if (ExpirationDate.HasValue)
            {
                query = query.Where(c => c.ExpirationDate <= ExpirationDate);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(c => c.Status == Status);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _couponRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Coupon).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(c => EF.Property<object>(c, sortBy))
                    : query.OrderByDescending(c => EF.Property<object>(c, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var coupons = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (coupons, paginationMetadata);
        }

        public async Task<Coupon> GetCouponByIdAsync(int id)
        {
            return await _couponRepository.Query()
                .Where(c => c.CouponId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Coupon> CreateCouponAsync(Coupon coupon)
        {
            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();
            return coupon;
        }

        public async Task<Coupon> UpdateCouponAsync(int id, Coupon coupon)
        {
            var existingCoupon = await _couponRepository.GetByIdAsync(id);
            if (existingCoupon == null)
            {
                return null;
            }

            existingCoupon.CouponCode = coupon.CouponCode;
            existingCoupon.DiscountAmount = coupon.DiscountAmount;
            existingCoupon.StartDate = coupon.StartDate;
            existingCoupon.ExpirationDate = coupon.ExpirationDate;
            existingCoupon.AmountItem = coupon.AmountItem;
            existingCoupon.Status = coupon.Status;

            await _couponRepository.UpdateAsync(existingCoupon);
            await _couponRepository.SaveChangesAsync();

            return existingCoupon;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return false;
            }

            await _couponRepository.DeleteAsync(id);
            await _couponRepository.SaveChangesAsync();

            return true;
        }
    }
}