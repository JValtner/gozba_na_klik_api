namespace Gozba_na_klik.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int RestaurantId { get; set; }

        //public Restaurant Restaurant { get; set; }
        public List<MealAddon> Addons { get; set; } = new List<MealAddon>();
        public List<Alergen> Alergens { get; set; } = new List<Alergen>();
    }
}
