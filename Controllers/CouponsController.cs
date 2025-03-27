using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<CouponDto>, PaginationMetadata)>> GetCoupons(
            [FromQuery] string? couponCode,
            [FromQuery] decimal? minDiscount,
            [FromQuery] decimal? maxDiscount,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? expirationDate,
            [FromQuery] int? status,
            [FromQuery] int? paymentId,
            [FromQuery] int? productId,
            [FromQuery] string sortBy = "CouponId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (coupons, pagination) = await _couponService.GetCouponsAsync(couponCode, minDiscount, maxDiscount, startDate, expirationDate, status, paymentId, productId, sortBy, isAscending, page, pageSize);
            return Ok(new { Coupons = coupons, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponDto>> GetCoupon(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
                return NotFound();

            return Ok(coupon);
        }

        [HttpPost]
        public async Task<ActionResult<CouponDto>> CreateCoupon([FromBody] CouponDto couponDto)
        {
            var createdCoupon = await _couponService.CreateCouponAsync(couponDto);
            return CreatedAtAction(nameof(GetCoupon), new { id = createdCoupon.CouponId }, createdCoupon);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CouponDto>> UpdateCoupon(int id, [FromBody] CouponDto couponDto)
        {
            if (id != couponDto.CouponId)
                return BadRequest("ID mismatch");

            var updatedCoupon = await _couponService.UpdateCouponAsync(id, couponDto);
            if (updatedCoupon == null)
                return NotFound();

            return Ok(updatedCoupon);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}