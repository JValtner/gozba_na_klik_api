using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Services;
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

        // GET: api/orders/{orderId}
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderById(
            int orderId,
            [FromHeader(Name = "X-User-Id")] int userId)
        {
            _logger.LogInformation("GET request for order {OrderId}", orderId);
            var order = await _orderService.GetOrderByIdAsync(userId, orderId);
            return Ok(order);
        }

        // GET: api/orders/restaurant/{restaurantId}
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<List<RestaurantOrderDto>>> GetRestaurantOrders(
            int restaurantId,
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromQuery] string? status = null)
        {
            _logger.LogInformation("GET request for restaurant {RestaurantId} orders", restaurantId);
            var orders = await _orderService.GetRestaurantOrdersAsync(userId, restaurantId, status);
            return Ok(orders);
        }

        // PUT: api/orders/{orderId}/accept
        [HttpPut("{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(
            int orderId,
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromBody] AcceptOrderDto dto)
        {
            _logger.LogInformation("PUT request to accept order {OrderId}", orderId);
            await _orderService.AcceptOrderAsync(userId, orderId, dto);
            return Ok(new { message = "Porudžbina prihvaćena." });
        }

        // PUT: api/orders/{orderId}/cancel
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(
            int orderId,
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromBody] CancelOrderDto dto)
        {
            _logger.LogInformation("PUT request to cancel order {OrderId}", orderId);
            await _orderService.CancelOrderAsync(userId, orderId, dto);
            return Ok(new { message = "Porudžbina otkazana." });
        }
        // GET: api/orders/user/{userId}
        [HttpGet("user/{userId}")]
            public async Task<ActionResult<PaginatedOrderHistoryResponseDto>> GetUserOrderHistory(
            int userId,
            [FromHeader(Name = "X-User-Id")] int requestingUserId,
            [FromQuery] string? statusFilter = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET request for order history of user {UserId} by requesting user {RequestingUserId}",
                userId, requestingUserId);

            if (userId != requestingUserId)
            {
                _logger.LogWarning("User {RequestingUserId} attempted to access orders of user {UserId}",
                    requestingUserId, userId);
                return Unauthorized(new { message = "Možete videti samo svoje porudžbine." });
            }

            if (page < 1)
                page = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var result = await _orderService.GetUserOrderHistoryAsync(userId, statusFilter, page, pageSize);
            return Ok(result);
        }

        // GET: api/orders/courier/{id}/active-pickup
        [HttpGet("courier/{courierId}/active-pickup")]
        public async Task<ActionResult<CourierActiveOrderDto>> GetCourierOrderInPickupAsync(int courierId)
        {
            _logger.LogInformation("GET zahtev za proveru da li je kuriru dodeljena dostava");
            return Ok(await _orderService.GetCourierOrderInPickupAsync(courierId));
        }

        // PUT: api/orders/{orderId}/status/to-in-delivery
        [HttpPut("{orderId}/status/to-in-delivery")]
        public async Task<ActionResult<OrderStatusDto>> UpdateOrderToInDeliveryAsync(int orderId)
        {
            _logger.LogInformation("PUT zahtev za promenu statusa narudzbine u 'DOSTAVA U TOKU'");
            return Ok(await _orderService.UpdateOrderToInDeliveryAsync(orderId));
        }

        // PUT: api/orders/{orderId}/status/to-delivered
        [HttpPut("{orderId}/status/to-delivered")]
        public async Task<ActionResult<OrderStatusDto>> UpdateOrderToDeliveredAsync(int orderId)
        {
            _logger.LogInformation("PUT zahtev za promenu statusa narudzbine u 'ZAVRSENO'");
            return Ok(await _orderService.UpdateOrderToDeliveredAsync(orderId));
        }

        // GET: api/orders/user/my-active-order
    }
}
