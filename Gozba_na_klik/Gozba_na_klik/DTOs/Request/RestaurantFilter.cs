using System.ComponentModel.DataAnnotations;

public class RestaurantFilter
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    public DateTime? CurrentDate { get; set; } = DateTime.UtcNow;
}