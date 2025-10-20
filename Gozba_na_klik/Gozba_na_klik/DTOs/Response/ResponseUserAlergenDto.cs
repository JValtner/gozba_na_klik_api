namespace Gozba_na_klik.DTOs.Response
{
    public class ResponseUserAlergenDto
    {
        public int Id { get; set; }
        public List<ResponseAlergenBasicDto> Alergens { get; set; } = new();
    }
}
