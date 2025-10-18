namespace Gozba_na_klik.Models
{
    public class DeliveryPersonSchedule
    {
        public int Id { get; set; }
        public int DeliveryPersonId { get; set; }
        public User DeliveryPerson { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}