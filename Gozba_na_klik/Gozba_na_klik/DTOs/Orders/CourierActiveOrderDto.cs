public class CourierActiveOrderDto
{
    public int OrderId { get; set; }
    public CourierOrderUserDto Buyer { get; set; } = null!;
    public CourierOrderRestaurantDto Restaurant { get; set; } = null!;
    public CourierOrderAddressDto Address { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal SubtotalPrice { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalPrice { get; set; }
    public string? CustomerNote { get; set; }
    public List<CourierOrderItemDto> Items { get; set; } = new();
}

public class CourierOrderUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
}

public class CourierOrderRestaurantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
}

public class CourierOrderAddressDto
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

public class CourierOrderItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
}
