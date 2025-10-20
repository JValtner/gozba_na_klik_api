namespace Gozba_na_klik.Models;

public class Review
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int RestaurantId { get; set; }
    public int CourierId { get; set; }
    public int RestaurantRating { get; set; } // 1-5
    public string? RestaurantComment { get; set; }
    public string? RestaurantPhotoUrl { get; set; }
    public int CourierRating { get; set; } // 1-5
    public string? CourierComment { get; set; }
    public DateTime CreatedAt { get; set; }
}
