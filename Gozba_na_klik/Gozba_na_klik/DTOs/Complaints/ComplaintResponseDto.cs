namespace Gozba_na_klik.DTOs.Complaints
{
    public class ComplaintResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

