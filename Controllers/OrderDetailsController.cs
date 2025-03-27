using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/order/{orderId}/detail")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<(IEnumerable<OrderDetailDto>, PaginationMetadata)>> GetOrderDetails(
            [FromQuery] int? orderId,
            [FromQuery] int? productId,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "OrderDetailId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (details, pagination) = await _orderDetailService.GetOrderDetailsAsync(orderId, productId, status, sortBy, isAscending, page, pageSize);
            return Ok(new { OrderDetails = details, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int id)
        {
            var detail = await _orderDetailService.GetOrderDetailByIdAsync(id);
            if (detail == null)
                return NotFound();

            return Ok(detail);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetailDto>> CreateOrderDetail([FromBody] OrderDetailDto detailDto)
        {
            var createdDetail = await _orderDetailService.CreateOrderDetailAsync(detailDto);
            return CreatedAtAction(nameof(GetOrderDetail), new { id = createdDetail.OrderDetailId }, createdDetail);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDetailDto>> UpdateOrderDetail(int id, [FromBody] OrderDetailDto detailDto)
        {
            if (id != detailDto.OrderDetailId)
                return BadRequest("ID mismatch");

            var updatedDetail = await _orderDetailService.UpdateOrderDetailAsync(id, detailDto);
            if (updatedDetail == null)
                return NotFound();

            return Ok(updatedDetail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var result = await _orderDetailService.DeleteOrderDetailAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}