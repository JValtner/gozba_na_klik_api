namespace Gozba_na_klik.DTOs.DeliveryPersonSchedule
{
    public class WeeklyScheduleDto
    {
        public List<DeliveryScheduleDto> Schedule { get; set; }
        public double TotalWeeklyHours { get; set; }
        public double RemainingHours { get; set; }
    }
}