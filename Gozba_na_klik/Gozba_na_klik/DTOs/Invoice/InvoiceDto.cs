namespace Gozba_na_klik.DTOs.Invoice
{
    public class InvoiceDto
    {
        public string InvoiceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public CustomerInfoDto Customer { get; set; } = new();
        public RestaurantInfoDto Restaurant { get; set; } = new();
        public AddressInfoDto DeliveryAddress { get; set; } = new();
        public List<InvoiceItemDto> Items { get; set; } = new();
        public InvoiceSummaryDto Summary { get; set; } = new();
        public string? CustomerNote { get; set; }
        public bool HasAllergenWarning { get; set; }
        public PaymentInfoDto Payment { get; set; } = new();
    }
}
