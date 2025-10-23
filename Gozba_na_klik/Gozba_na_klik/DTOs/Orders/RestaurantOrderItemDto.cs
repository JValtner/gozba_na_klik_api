namespace Gozba_na_klik.DTOs.Orders
{
    public class RestaurantOrderItemDto
    {
        public string MealName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string>? SelectedAddons { get; set; }
    }
}