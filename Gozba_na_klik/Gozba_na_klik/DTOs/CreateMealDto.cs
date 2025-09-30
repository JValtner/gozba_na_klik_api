namespace Gozba_na_klik.DTOs
{
    public class CreateMealDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int RestaurantId { get; set; }
    }
}
