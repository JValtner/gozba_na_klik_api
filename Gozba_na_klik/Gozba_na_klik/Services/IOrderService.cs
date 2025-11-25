using System;   
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Services
{
    public interface IOrderService
    {
        // Kupac
        Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderDetailsDto> GetOrderByIdAsync(int userId, int orderId);
        Task<OrderStatusResponseDto> GetActiveOrderStatusAsync(int userId);
        Task<PaginatedOrderHistoryResponseDto> GetUserOrderHistoryAsync(
            int userId, int requestingUserId, string? statusFilter, int page, int pageSize);

        // Restoran
        Task<List<RestaurantOrderDto>> GetRestaurantOrdersAsync(int userId, int restaurantId, string? status = null);
        Task AcceptOrderAsync(int userId, int orderId, AcceptOrderDto dto);
        Task CancelOrderAsync(int userId, int orderId, CancelOrderDto dto);
        // Kurir
        Task<List<Order>> GetAllAcceptedOrdersAsync();
        Task AssignCourierToOrderAsync(int orderId, int courierId);
        Task<CourierActiveOrderDto?> GetCourierOrderInPickupAsync(int courierId);
        Task<OrderStatusDto?> UpdateOrderToInDeliveryAsync(int orderId);
        Task<OrderStatusDto?> UpdateOrderToDeliveredAsync(int orderId);
        Task<CourierDeliveryHistoryResponseDto> GetCourierDeliveryHistoryAsync(
          int courierId,
          int requestingCourierId,
          DateTime? fromDate,
          DateTime? toDate,
          int page,
          int pageSize);
    }
}
