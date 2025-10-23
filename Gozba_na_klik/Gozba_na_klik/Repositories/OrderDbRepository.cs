using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Repositories
{
    public class OrderDbRepository : IOrderRepository
    {
        private readonly GozbaNaKlikDbContext _context;
        private readonly ILogger<OrderDbRepository> _logger;

        public OrderDbRepository(GozbaNaKlikDbContext context, ILogger<OrderDbRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.ChangeTracker.Clear();

            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} saved successfully", order.Id);

                var createdOrder = await _context.Orders
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(o => o.User)
                    .Include(o => o.Restaurant)
                    .Include(o => o.Address)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Meal)
                    .FirstAsync(o => o.Id == order.Id);

                return createdOrder;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving order: {Message}", ex.InnerException?.Message ?? ex.Message);
                throw new Exception($"Error saving order: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetRestaurantOrdersAsync(int restaurantId, string? status = null)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                .Where(o => o.RestaurantId == restaurantId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}