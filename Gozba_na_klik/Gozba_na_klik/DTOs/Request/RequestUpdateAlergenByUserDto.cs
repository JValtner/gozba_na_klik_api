using System.ComponentModel.DataAnnotations;
using Gozba_na_klik.DTOs.Response;

namespace Gozba_na_klik.DTOs.Request
{
    public class RequestUpdateAlergenByUserDto
    {
        public int Id { get; set; }
        public List<int> AlergensIds { get; set; } = new();
    }
}
