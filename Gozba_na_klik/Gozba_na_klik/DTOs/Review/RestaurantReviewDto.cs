namespace Gozba_na_klik.DTOs.Review;
public class RestaurantReviewDto
{
    public int RestaurantRating { get; set; }
    public string? RestaurantComment { get; set; }
    public string? RestaurantPhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
