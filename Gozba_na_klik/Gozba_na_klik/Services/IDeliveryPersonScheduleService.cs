using Gozba_na_klik.DTOs.DeliveryPersonSchedule;

namespace Gozba_na_klik.Services
{
    public interface IDeliveryPersonScheduleService
    {
        Task<WeeklyScheduleDto> GetWeeklyScheduleAsync(int deliveryPersonId);
        Task<DeliveryScheduleDto> CreateScheduleAsync(int deliveryPersonId, CreateDeliveryScheduleDto dto);
        Task<DeliveryScheduleDto> UpdateScheduleAsync(int deliveryPersonId, int scheduleId, CreateDeliveryScheduleDto dto);
        Task DeleteScheduleAsync(int deliveryPersonId, int scheduleId);
    }
}