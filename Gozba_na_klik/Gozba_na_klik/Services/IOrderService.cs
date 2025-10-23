using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Services
{
    public interface IOrderService
    {
        Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<PaginatedOrderHistoryResponseDto> GetUserOrderHistoryAsync(int userId, string? statusFilter, int page, int pageSize);
        Task<Order?> GetCourierOrderInPickupAsync(int courierId);
        Task<Order?> UpdateOrderToInDeliveryAsync(int orderId);
        Task<Order?> UpdateOrderToDeliveredAsync(int orderId);
    }
}
