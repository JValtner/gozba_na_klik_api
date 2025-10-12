namespace Gozba_na_klik.Models
{
    public class MealAddon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; } // e.g., "indipendant", "chosen" only
        public int MealId { get; set; } 
        public Meal Meal { get; set; }
        public bool IsActive { get; set; } = false; //only relevant for chosen type
    }
}
