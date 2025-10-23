using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Services
{
    public interface IOrderService
    {
        // Kupac
        Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderDetailsDto> GetOrderByIdAsync(int userId, int orderId);

        // Restoran
        Task<List<RestaurantOrderDto>> GetRestaurantOrdersAsync(int userId, int restaurantId, string? status = null);
        Task AcceptOrderAsync(int userId, int orderId, AcceptOrderDto dto);
        Task CancelOrderAsync(int userId, int orderId, CancelOrderDto dto);

        // Kupac
        Task<PaginatedOrderHistoryResponseDto> GetUserOrderHistoryAsync(
            int userId, string? statusFilter, int page, int pageSize);

        // Kurir
        Task<CourierActiveOrderDto?> GetCourierOrderInPickupAsync(int courierId);
        Task<OrderStatusDto?> UpdateOrderToInDeliveryAsync(int orderId);
        Task<OrderStatusDto?> UpdateOrderToDeliveredAsync(int orderId);
    }
}
