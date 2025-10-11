namespace Gozba_na_klik.Models;

public class ClosedDate
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}
