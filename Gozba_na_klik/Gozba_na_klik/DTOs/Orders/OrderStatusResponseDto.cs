namespace Gozba_na_klik.DTOs.Orders
{
    public class OrderStatusResponseDto
    {
        public int OrderId { get; set; }
        public string? Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? LastUpdated { get; set; }

        public CustomerAddressDto? CustomerAddress { get; set; }
        public RestaurantDto? Restaurant { get; set; }
        public DeliveryPersonDto? DeliveryPerson { get; set; }

        public decimal SubtotalPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }

        public string? CustomerNote { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();

    }

    public class DeliveryPersonDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
    }

    public class RestaurantDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }

    public class CustomerAddressDto
    { 
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Entrance { get; set; }
        public string? Floor { get; set; }
        public string? Apartment { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SelectedAddons { get; set; }
    }
}
