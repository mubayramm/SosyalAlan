namespace SosyalAlan.Models
{
    public class Mesaj
    {
        public int Id { get; set; }
        public int GonderenId { get; set; }
        public int AlanId { get; set; }
        public string Icerik { get; set; }
        public DateTime Tarih { get; set; }
        public bool OkunduMu { get; set; } // false ise okunmadı true ise okundu.

        public Kullanici Gonderen { get; set; }
        public Kullanici Alan { get; set; }
    }
}