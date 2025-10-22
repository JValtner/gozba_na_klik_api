using System.ComponentModel.DataAnnotations;

public class MealFilter
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinPrice { get; set; } = 0;

    [Range(0, double.MaxValue)]
    public decimal? MaxPrice { get; set; } = 0;

    [MaxLength(100)]
    public string? RestaurantName { get; set; }

    public List<string>? Alergens { get; set; } = new();
    public List<string>? Addons { get; set; } = new();
}