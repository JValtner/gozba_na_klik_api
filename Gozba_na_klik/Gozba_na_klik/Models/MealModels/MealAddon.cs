namespace Gozba_na_klik.Models.MealModels
{
    public class MealAddon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int MealId { get; set; } 
        public Meal Meal { get; set; } 
    }
}
