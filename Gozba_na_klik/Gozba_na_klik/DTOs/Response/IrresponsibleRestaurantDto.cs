namespace Gozba_na_klik.DTOs.Response
{
    public class IrresponsibleRestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? OwnerUsername { get; set; }
        public int CancelledOrdersCount { get; set; }
        public bool IsSuspended { get; set; }
        public string? SuspensionStatus { get; set; }
    }
}

