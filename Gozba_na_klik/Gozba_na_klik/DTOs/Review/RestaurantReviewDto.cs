namespace Gozba_na_klik.DTOs.Review;
public class RestaurantReviewDto
{
    public int Id { get; set; }
    public int RestaurantRating { get; set; }
    public string? RestaurantComment { get; set; }
    public string? RestaurantPhotoUrl { get; set; }
    public int CourierRating { get; set; }
    public string? CourierComment { get; set; }
    public DateTime CreatedAt { get; set; }
}
