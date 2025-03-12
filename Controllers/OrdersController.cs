using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services;
using WebApplication2.Models;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<(IEnumerable<OrderDto>, PaginationMetadata)>> GetOrders(
            [FromQuery] int? orderId,
            [FromQuery] DateTime? orderDate,
            [FromQuery] string? orderDescription,
            [FromQuery] decimal? totalAmount,
            [FromQuery] long CustomerId,
            [FromQuery] int? status,
            [FromQuery] string sortBy = "OrderId",
            [FromQuery] bool isAscending = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (orders, pagination) = await _orderService.GetOrdersAsync(
                orderId, orderDate, orderDescription, totalAmount, status,CustomerId,
                sortBy, isAscending, page, pageSize);

            return Ok(new { Orders = orders, Pagination = pagination });
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderDto orderDto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderId }, createdOrder);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            if (id != orderDto.OrderId)
            {
                return BadRequest("ID mismatch");
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);
            if (updatedOrder == null)
            {
                return NotFound();
            }

            return Ok(updatedOrder);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}