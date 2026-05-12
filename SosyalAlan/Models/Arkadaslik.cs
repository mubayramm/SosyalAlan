namespace SosyalAlan.Models
{
    public class Arkadaslik
    {
        public int Id { get; set; }
        public int GonderenId { get; set; }
        public int AlanId { get; set; }
        public string Durum { get; set; }  
        public DateTime GonderimTarihi { get; set; }

        public Kullanici Gonderen { get; set; }
        public Kullanici Alan { get; set; }
    }
}