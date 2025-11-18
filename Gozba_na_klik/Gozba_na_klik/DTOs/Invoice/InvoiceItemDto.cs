namespace Gozba_na_klik.DTOs.Invoice
{
    public class InvoiceItemDto
    {
        public int MealId { get; set; }
        public string MealName { get; set; } = string.Empty;
        public string? MealImagePath { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<InvoiceAddonDto> SelectedAddons { get; set; } = new();
    }
}
