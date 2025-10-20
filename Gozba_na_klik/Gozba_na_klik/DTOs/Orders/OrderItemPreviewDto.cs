namespace Gozba_na_klik.DTOs.Orders
{
    public class OrderItemPreviewDto
    {
        public int MealId { get; set; }
        public string MealName { get; set; }
        public string? MealImagePath { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<AddonPreviewDto> SelectedAddons { get; set; } = new();
    }
}
