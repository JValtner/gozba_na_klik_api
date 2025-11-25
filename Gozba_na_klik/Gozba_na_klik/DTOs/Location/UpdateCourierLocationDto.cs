namespace Gozba_na_klik.DTOs.Location
{
    public class UpdateCourierLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ActiveOrderId { get; set; }
    }
}
