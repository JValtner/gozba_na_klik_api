namespace Gozba_na_klik.Models.Restaurants;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Models.RestaurantModels;

public class Restaurant
{
    public int Id { get; set; }                     
    public string Name { get; set; }
    public string PhotoUrl { get; set; }
    public int OwnerId { get; set; }                 
    public User Owner { get; set; } 
    public string Description { get; set; }
    public List<Meal> Menu { get; set; } = new();

    // Standardno radno vreme po danima
    public List<WorkSchedule> WorkSchedules { get; set; } = new();

    // Neradni datumi
    public List<ClosedDate> ClosedDates { get; set; } = new();       

    // Mozda cemo morati da menjamo sa User na Employee zbog nekih suspenzija...
    public List<User> Employees { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
