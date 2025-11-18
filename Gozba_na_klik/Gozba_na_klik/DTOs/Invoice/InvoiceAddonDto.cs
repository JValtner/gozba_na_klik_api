namespace Gozba_na_klik.DTOs.Invoice
{
    public class InvoiceAddonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
