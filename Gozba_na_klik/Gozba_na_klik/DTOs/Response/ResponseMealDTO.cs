using Gozba_na_klik.Models;

namespace Gozba_na_klik.DTOs.Response
{
    public class ResponseMealDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; } = null;
        public Restaurant Restaurant { get; set; }
        public List<MealAddon> Addons { get; set; } = new List<MealAddon>();
        public List<Alergen> Alergens { get; set; } = new List<Alergen>();
    }
}
