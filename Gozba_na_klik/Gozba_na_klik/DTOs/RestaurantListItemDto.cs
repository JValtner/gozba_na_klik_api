namespace Gozba_na_klik.DTOs
{
    public class RestaurantListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public int OwnerId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }
}
