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

        // Dodeli dostavljaca dostave
        public async Task<Order?> AssignCourierToOrder(Order order, User courier)
        {
            order.DeliveryPersonId = courier.Id;
            order.Status = "PREUZIMANJE U TOKU";
            
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> ExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.Id == orderId);
        }
    }
}