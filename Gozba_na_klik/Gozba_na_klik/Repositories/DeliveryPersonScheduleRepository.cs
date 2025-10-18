using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories
{
    public class DeliveryPersonScheduleRepository : IDeliveryPersonScheduleRepository
    {
        private readonly GozbaNaKlikDbContext _context;

        public DeliveryPersonScheduleRepository(GozbaNaKlikDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryPersonSchedule?> GetByIdAsync(int id)
        {
            return await _context.DeliveryPersonSchedules
                .Include(s => s.DeliveryPerson)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<DeliveryPersonSchedule>> GetByDeliveryPersonAsync(int deliveryPersonId)
        {
            return await _context.DeliveryPersonSchedules
                .Where(s => s.DeliveryPersonId == deliveryPersonId)
                .OrderBy(s => s.DayOfWeek)
                .ToListAsync();
        }

        public async Task<DeliveryPersonSchedule?> GetByDeliveryPersonAndDayAsync(int deliveryPersonId, DayOfWeek dayOfWeek)
        {
            return await _context.DeliveryPersonSchedules
                .FirstOrDefaultAsync(s => s.DeliveryPersonId == deliveryPersonId && s.DayOfWeek == dayOfWeek);
        }

        public async Task<DeliveryPersonSchedule> AddAsync(DeliveryPersonSchedule schedule)
        {
            _context.DeliveryPersonSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<DeliveryPersonSchedule> UpdateAsync(DeliveryPersonSchedule schedule)
        {
            _context.DeliveryPersonSchedules.Update(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule == null) return false;

            _context.DeliveryPersonSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}