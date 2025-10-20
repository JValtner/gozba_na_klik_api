namespace Gozba_na_klik.Models
{
    public class UserAlergen
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int AlergenId { get; set; }
        public Alergen Alergen { get; set; }
    }
}
