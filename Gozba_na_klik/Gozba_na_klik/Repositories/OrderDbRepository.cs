using System;
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
                //.AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // ✅ Kreiranje porudžbine (tvoja verzija)
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

        // ✅ Lista porudžbina restorana
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

        // ✅ Ažuriranje porudžbine
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // ✅ Sve PRIHVAĆENE porudžbine bez dostavljača
        public async Task<List<Order>> GetAllAcceptedOrdersAsync()
        {
            return await _context.Orders
                .Where(order => order.Status == "PRIHVAĆENA" && order.DeliveryPersonId == null)
                .ToListAsync();
        }

        // ✅ Porudžbina trenutno u toku preuzimanja za određenog kurira
        // Vraća samo aktivne porudžbine (PREUZIMANJE U TOKU ili DOSTAVA U TOKU), ne završene
        public async Task<Order?> GetCourierOrderInPickupAsync(int courierId)
        {
            var activeStatuses = new[]
            {
                "PRIHVAĆENA",
                "PREUZIMANJE U TOKU",
                "DOSTAVA U TOKU"
            };

            return await _context.Orders
                .AsNoTracking()
                .Include(order => order.Restaurant)
                .Include(order => order.Items)
                    .ThenInclude(item => item.Meal)
                .Include(order => order.User)
                .Include(order => order.Address)
                .Where(order => order.DeliveryPersonId == courierId && activeStatuses.Contains(order.Status))
                .FirstOrDefaultAsync();
        }

        // ✅ Dohvatanje porudžbina po korisniku
        public async Task<(List<Order> Orders, int TotalCount)> GetOrdersByUserIdAsync(int userId, string? statusFilter, int page, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                .Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper() == "ACTIVE")
                {
                    query = query.Where(o => o.Status != OrderStatus.ISPORUČENA && o.Status != OrderStatus.OTKAZANA);
                }
                else if (statusFilter.ToUpper() == "ARCHIVED")
                {
                    query = query.Where(o => o.Status == OrderStatus.ISPORUČENA || o.Status == OrderStatus.OTKAZANA);
                }
                else
                {
                    query = query.Where(o => o.Status == statusFilter);
                }
            }

            query = query.OrderByDescending(o => o.OrderDate);

            var totalCount = await query.CountAsync();

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        // Dohvatanje aktivne porudzbine za korisnika
        public async Task<Order?> GetActiveOrderStatusAsync(int userId)
        {
            var activeStatuses = new[]
            {
                "NA_CEKANJU",
                "PRIHVAĆENA",
                "PREUZIMANJE U TOKU",
                "DOSTAVA U TOKU"
            };

            return await _context.Orders
                .Include(order => order.Restaurant)
                .Include(order => order.Address)
                .Include(order => order.DeliveryPerson)
                .Include(order => order.Items)
                    .ThenInclude(item => item.Meal)
                .Where(order => order.UserId == userId && activeStatuses.Contains(order.Status))
                .OrderByDescending(order => order.OrderDate)
                .FirstOrDefaultAsync();
        }
        public async Task<(List<Order> Orders, int TotalCount)> GetCourierDeliveriesAsync(
          int courierId,
          DateTime? fromDate,
          DateTime? toDate,
          int page,
          int pageSize)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(order => order.Restaurant)
                .Where(order => order.DeliveryPersonId == courierId && order.Status == "ZAVRŠENO");

            if (fromDate.HasValue)
            {
                query = query.Where(order => order.DeliveryTime >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(order => order.DeliveryTime <= toDate.Value);
            }

            var totalCount = await query.CountAsync();

            var deliveries = await query
                .OrderByDescending(order => order.DeliveryTime ?? order.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (deliveries, totalCount);
        }
        public async Task<List<int>> GetTop5PopularMealIdsAsync()
        {
            return await _context.OrderItems
                .GroupBy(oi => oi.MealId)
                .Select(g => new
                {
                    MealId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .Select(x => x.MealId)
                .ToListAsync();
        }


    }
}
