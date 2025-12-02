using Gozba_na_klik.Models;

namespace Gozba_na_klik.DTOs.Response
{
    public class ResponseRestaurantDTO
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public List<Meal> Menu { get; set; } = new();

        // Standardno radno vreme po danima
        public bool isOpen { get; set; } =false;
        public List<WorkSchedule> WorkSchedules { get; set; } = new();

        // Neradni datumi
        public List<ClosedDate> ClosedDates { get; set; } = new();

        // Suspension status
        public bool IsSuspended { get; set; } = false;
        public string? SuspensionStatus { get; set; }
        public string? SuspensionReason { get; set; }

    }
}
