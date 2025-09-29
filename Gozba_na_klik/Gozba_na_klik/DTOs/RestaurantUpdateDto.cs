namespace Gozba_na_klik.DTOs
{
	public class RestaurantUpdateDto
	{
		public string Name { get; set; }
		public string? PhotoUrl { get; set; }
												  
		public string? Address { get; set; }
		public string? Description { get; set; }
		public string? Phone { get; set; }     
	}
}
