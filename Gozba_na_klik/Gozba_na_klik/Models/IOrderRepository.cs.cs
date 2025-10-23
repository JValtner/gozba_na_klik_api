using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId);
        Task<List<Order>> GetAllAcceptedOrdersAsync();
        Task<Order?> GetCourierOrderInPickupAsync(int courierId);
        Task<Order> AddAsync(Order order);
        Task<Order?> AssignCourierToOrderAsync(Order order, User courier);
        Task<Order?> UpdateOrderStatusAsync(Order order);
        Task<bool> ExistsAsync(int orderId);
        Task<(List<Order> Orders, int TotalCount)> GetOrdersByUserIdAsync(int userId, string? statusFilter, int page, int pageSize);
    }
}
