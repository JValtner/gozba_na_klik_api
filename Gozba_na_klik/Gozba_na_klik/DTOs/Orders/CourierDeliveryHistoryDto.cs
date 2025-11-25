namespace Gozba_na_klik.DTOs.Orders
{
	public class CourierDeliveryHistoryItemDto
	{
		public int OrderId { get; set; }
		public string RestaurantName { get; set; } = string.Empty;
		public DateTime? PickupTime { get; set; }
		public DateTime? DeliveryTime { get; set; }
		public int? DurationMinutes { get; set; }
		public decimal TotalPrice { get; set; }
	}

	public class CourierDeliveryHistoryResponseDto
	{
		public List<CourierDeliveryHistoryItemDto> Deliveries { get; set; } = new();
		public int TotalCount { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
		public bool HasNextPage => Page < TotalPages;
		public bool HasPreviousPage => Page > 1;
	}
}
