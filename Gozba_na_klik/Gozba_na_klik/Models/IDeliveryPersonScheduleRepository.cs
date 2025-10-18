namespace Gozba_na_klik.Models
{
    public interface IDeliveryPersonScheduleRepository
    {
        Task<DeliveryPersonSchedule?> GetByIdAsync(int id);
        Task<List<DeliveryPersonSchedule>> GetByDeliveryPersonAsync(int deliveryPersonId);
        Task<DeliveryPersonSchedule?> GetByDeliveryPersonAndDayAsync(int deliveryPersonId, DayOfWeek dayOfWeek);
        Task<DeliveryPersonSchedule> AddAsync(DeliveryPersonSchedule schedule);
        Task<DeliveryPersonSchedule> UpdateAsync(DeliveryPersonSchedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}