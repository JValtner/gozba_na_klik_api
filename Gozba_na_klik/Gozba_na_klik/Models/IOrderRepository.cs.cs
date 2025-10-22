using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId);
        Task<List<Order>> GetAllAcceptedOrdersAsync();
        Task<Order> AddAsync(Order order);
        Task<Order?> AssignCourierToOrder(Order order, User courier);
        Task<bool> ExistsAsync(int orderId);
        Task<(List<Order> Orders, int TotalCount)> GetOrdersByUserIdAsync(int userId, string? statusFilter, int page, int pageSize);
    }
}
