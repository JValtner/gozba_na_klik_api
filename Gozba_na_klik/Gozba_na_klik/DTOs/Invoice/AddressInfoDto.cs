namespace Gozba_na_klik.DTOs.Invoice
{
    public class AddressInfoDto
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? Entrance { get; set; }
        public string? Floor { get; set; }
        public string? Apartment { get; set; }
    }
}
