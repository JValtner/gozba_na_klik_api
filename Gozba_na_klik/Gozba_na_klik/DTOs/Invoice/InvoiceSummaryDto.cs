namespace Gozba_na_klik.DTOs.Invoice
{
    public class InvoiceSummaryDto
    {
        public decimal SubtotalPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
    }
}
