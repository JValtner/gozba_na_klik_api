namespace Gozba_na_klik.DTOs.Invoice
{
    public class RestaurantInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
