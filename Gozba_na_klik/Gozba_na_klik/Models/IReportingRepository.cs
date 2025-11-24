using Gozba_na_klik.Models.Orders;

public interface IReportingRepository
{
    Task<List<Order>> GetOrdersForPeriod(int restaurantId, DateTime startDate, DateTime endDate);
    Task<List<OrderItem>> GetMealSalesForPeriod(int restaurantId, int mealId, DateTime startDate, DateTime endDate);
}