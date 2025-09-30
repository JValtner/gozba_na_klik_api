using Gozba_na_klik.Models.Restaurants;

namespace Gozba_na_klik.Models.RestaurantModels;

public class ClosedDate
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}
