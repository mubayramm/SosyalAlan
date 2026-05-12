namespace SosyalAlan.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Eposta { get; set; }
        public string SifreHash { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}