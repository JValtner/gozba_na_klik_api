namespace Gozba_na_klik.Models
{
    public class Alergen
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MealId { get; set; }
        public Meal Meal { get; set; }
    }
}
