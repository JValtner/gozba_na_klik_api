namespace Gozba_na_klik.DTOs.Response
{
    public class SuspensionResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string? RestaurantName { get; set; }
        public string SuspensionReason { get; set; } = string.Empty;
        public DateTime SuspendedAt { get; set; }
        public int SuspendedBy { get; set; }
        public string Status { get; set; } = "SUSPENDED";
        public string? AppealText { get; set; }
        public DateTime? AppealDate { get; set; }
        public DateTime? DecisionDate { get; set; }
        public int? DecisionBy { get; set; }
    }
}

