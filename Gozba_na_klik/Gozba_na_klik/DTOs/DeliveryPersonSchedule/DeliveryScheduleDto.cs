namespace Gozba_na_klik.DTOs.DeliveryPersonSchedule
{
    public class DeliveryScheduleDto
    {
        public int Id { get; set; }
        public int DayOfWeek { get; set; }
        public string DayName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double Hours { get; set; }
        public bool IsActive { get; set; }
    }
}