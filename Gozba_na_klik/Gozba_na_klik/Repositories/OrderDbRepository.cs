using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories
{
    public class OrderDbRepository : IOrderRepository
    {
        private readonly GozbaNaKlikDbContext _context;

        public OrderDbRepository(GozbaNaKlikDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // Sve sa statusom PRIHVACENA
        public async Task<List<Order>> GetAllAcceptedOrdersAsync()
        {
            return await _context.Orders
                .Include(order => order.User)
                .Include(order => order.Restaurant)
                .Include(order => order.Address)
                .Include(order => order.Items)
                .Where(order => order.Status == "PRIHVAĆENA" && order.DeliveryPersonId == null)
                .ToListAsync();
        }

        // Sve dostave sa statusom PREUZIMANJE U TOKU
        // BITNO OVO SE AKTIVIRA OD FRONTA NA 10 SEK
        public async Task<Order?> GetCourierOrderInPickupAsync(int courierId)
        {
            return await _context.Orders
                .Include(order => order.Restaurant)
                .Include(order => order.Items)
                .Include(order => order.User)
                .Include(order => order.Address)
                .Where(order => order.DeliveryPersonId == courierId && order.Status != "PRIHVAĆENA")
                .FirstOrDefaultAsync();
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.ChangeTracker.Clear();

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _context.Entry(order).State = EntityState.Detached;

            var createdOrder = await _context.Orders
                .AsSplitQuery()
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                .FirstAsync(o => o.Id == order.Id);

            return createdOrder;
        }

        // Dodeli dostavi dostavljaca
        public async Task<Order?> AssignCourierToOrderAsync(Order order, User courier)
        {
            order.DeliveryPersonId = courier.Id;
            order.Status = "PREUZIMANJE U TOKU";
            
            await _context.SaveChangesAsync();
            return order;
        }

        // Promeni status dostave:
        public async Task<Order?> UpdateOrderStatusAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<bool> ExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.Id == orderId);
        }

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
    }
}