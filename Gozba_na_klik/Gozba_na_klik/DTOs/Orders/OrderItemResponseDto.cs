namespace Gozba_na_klik.DTOs.Orders
{
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public string MealName { get; set; }
        public string? MealImagePath { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string>? SelectedAddons { get; set; }
    }
}
