namespace Gozba_na_klik.DTOs.Invoice
{
    public class PaymentInfoDto
    {
        public string Method { get; set; } = "Gotovina"; // Default to cash
        public string Status { get; set; } = "Plaćeno";
        public DateTime? PaymentDate { get; set; }
    }
}
