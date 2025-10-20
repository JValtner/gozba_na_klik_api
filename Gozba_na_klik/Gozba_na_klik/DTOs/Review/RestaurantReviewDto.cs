namespace Gozba_na_klik.Models;

public class RestaurantReviewDto
{
    public int RestaurantRating { get; set; }
    public string? RestaurantComment { get; set; }
    public string? RestaurantPhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
