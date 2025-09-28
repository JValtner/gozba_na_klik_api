namespace Gozba_na_klik.Models.Restaurants;

public class ClosedDate
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}
