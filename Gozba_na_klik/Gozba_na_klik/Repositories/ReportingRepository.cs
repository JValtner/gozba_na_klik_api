using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.EntityFrameworkCore;

public class ReportingRepository : IReportingRepository
{
    private readonly GozbaNaKlikDbContext _context;

    public ReportingRepository(GozbaNaKlikDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetOrdersForPeriod(int restaurantId, DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.RestaurantId == restaurantId &&
                        o.OrderDate >= startDate &&
                        o.OrderDate <= endDate)
            .ToListAsync();
    }

    public async Task<List<OrderItem>> GetMealSalesForPeriod(int restaurantId, int mealId, DateTime startDate, DateTime endDate)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order.RestaurantId == restaurantId &&
                         oi.MealId == mealId &&
                         oi.Order.OrderDate >= startDate &&
                         oi.Order.OrderDate <= endDate)
            .ToListAsync();
    }
}