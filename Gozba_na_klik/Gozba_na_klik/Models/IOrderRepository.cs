using System;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public interface IOrderRepository
    {
         Task<Order?> GetByIdAsync(int orderId);
        Task<Order> AddAsync(Order order);
        Task<bool> ExistsAsync(int orderId);
        Task<List<Order>> GetRestaurantOrdersAsync(int restaurantId, string? status = null);
        Task UpdateAsync(Order order);
        Task<List<Order>> GetAllAcceptedOrdersAsync();
        Task<Order?> GetCourierOrderInPickupAsync(int courierId);
        Task<(List<Order> Orders, int TotalCount)> GetOrdersByUserIdAsync(
            int userId, string? statusFilter, int page, int pageSize
        );
        Task<Order?> GetActiveOrderStatusAsync(int userId);
        Task<(List<Order> Orders, int TotalCount)> GetCourierDeliveriesAsync(
            int courierId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize);
    }
}
