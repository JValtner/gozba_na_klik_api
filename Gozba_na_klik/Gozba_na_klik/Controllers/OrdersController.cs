using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // POST: api/orders/preview/restaurant/{restaurantId}
        [HttpPost("preview/restaurant/{restaurantId}")]
        public async Task<ActionResult<OrderPreviewDto>> GetOrderPreview(
            int restaurantId,
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromBody] CreateOrderDto dto)
        {
            _logger.LogInformation("POST request for order preview at restaurant {RestaurantId}", restaurantId);
            var preview = await _orderService.GetOrderPreviewAsync(userId, restaurantId, dto);
            return Ok(preview);
        }

        // POST: api/orders/restaurant/{restaurantId}
        [HttpPost("restaurant/{restaurantId}")]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(
            int restaurantId,
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromBody] CreateOrderDto dto)
        {
            _logger.LogInformation("POST request to create order for restaurant {RestaurantId}", restaurantId);
            var order = await _orderService.CreateOrderAsync(userId, restaurantId, dto);
            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, order);
        }
    }
}
