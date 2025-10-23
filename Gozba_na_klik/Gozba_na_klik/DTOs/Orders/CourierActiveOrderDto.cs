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
}

public class CourierOrderAddressDto
{
    public int Id { get; set; }
    public string Street { get; set; } = null!;
}

public class CourierOrderItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
}
