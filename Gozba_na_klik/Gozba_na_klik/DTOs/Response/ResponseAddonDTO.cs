using Gozba_na_klik.Models;

namespace Gozba_na_klik.DTOs.Response
{
    public class ResponseAddonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int MealId { get; set; }
    }
}
