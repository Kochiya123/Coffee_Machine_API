using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail(OrderDetail orderDetail)
        {
            var createdOrderDetail = await _orderDetailService.CreateOrderDetailAsync(orderDetail);
            return CreatedAtAction(nameof(GetOrderDetailById), new { id = createdOrderDetail.OrderDetailId }, createdOrderDetail);
        }

        [HttpPut("{id}/detail")]
        public async Task<IActionResult> UpdateOrderDetail(int id, OrderDetail orderDetail)
        {
            var updatedOrderDetail = await _orderDetailService.UpdateOrderDetailAsync(id, orderDetail);
            if (updatedOrderDetail == null)
            {
                return NotFound();
            }
            return Ok(updatedOrderDetail);
        }

        [HttpDelete("{id}/detail")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var result = await _orderDetailService.DeleteOrderDetailAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}