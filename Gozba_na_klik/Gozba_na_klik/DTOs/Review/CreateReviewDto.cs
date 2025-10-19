namespace Gozba_na_klik.Models;

public class CreateReviewDto
{
    public int OrderId { get; set; }
    public int RestaurantRating { get; set; }
    public string? RestaurantComment { get; set; }
    public IFormFile? RestaurantPhoto { get; set; }
    public int CourierRating { get; set; }
    public string? CourierComment { get; set; }
}
