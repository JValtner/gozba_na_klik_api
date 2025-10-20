namespace Gozba_na_klik.Models.Orders
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int MealId { get; set; }
        public Meal Meal { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SelectedAddons { get; set; }
    }
}
