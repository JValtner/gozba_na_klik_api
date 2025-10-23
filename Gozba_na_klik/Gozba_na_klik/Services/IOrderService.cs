using Gozba_na_klik.DTOs.Orders;

namespace Gozba_na_klik.Services
{
    public interface IOrderService
    {
        Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderDetailsDto> GetOrderByIdAsync(int userId, int orderId);
        Task<List<RestaurantOrderDto>> GetRestaurantOrdersAsync(int userId, int restaurantId, string? status = null);
        Task AcceptOrderAsync(int userId, int orderId, AcceptOrderDto dto);
        Task CancelOrderAsync(int userId, int orderId, CancelOrderDto dto);
    }
}
