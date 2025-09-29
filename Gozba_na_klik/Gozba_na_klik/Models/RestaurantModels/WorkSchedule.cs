using Gozba_na_klik.Models.Restaurants;

namespace Gozba_na_klik.Models.RestaurantModels;

public class WorkSchedule
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }

}
