using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupons(
            [FromQuery] int? CouponId, [FromQuery] string? CouponCode, [FromQuery] decimal? DiscountAmount,
            [FromQuery] DateTime? StartDate, [FromQuery] DateTime? ExpirationDate, [FromQuery] int? Status,
            [FromQuery] string sortBy = "CouponId", [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (coupons, pagination) = await _couponService.GetCouponsAsync(CouponId, CouponCode, DiscountAmount, StartDate, ExpirationDate, Status, sortBy, isAscending, page, pageSize);
            return Ok(new { Coupons = coupons, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(Coupon coupon)
        {
            var createdCoupon = await _couponService.CreateCouponAsync(coupon);
            return CreatedAtAction(nameof(GetCouponById), new { id = createdCoupon.CouponId }, createdCoupon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, Coupon coupon)
        {
            var updatedCoupon = await _couponService.UpdateCouponAsync(id, coupon);
            if (updatedCoupon == null)
            {
                return NotFound();
            }
            return Ok(updatedCoupon);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}