using Microsoft.EntityFrameworkCore;
using SosyalAlan.Data;
using SosyalAlan.DTOs;
using SosyalAlan.Models;

namespace SosyalAlan.Services
{
    public class MesajService
    {
        private readonly AppDbContext _context;

        public MesajService(AppDbContext context)
        {
            _context = context;
        }

        // Mesaj gönder
        public async Task<string> MesajGonder(int gonderenId, MesajGonderDto dto)
        {
            if (gonderenId == dto.AlanId)
                return "Kendinize mesaj gönderemezsiniz.";

            bool aliciVarMi = await _context.Kullanicilar.AnyAsync(k => k.Id == dto.AlanId);
            if (!aliciVarMi)
                return "Alıcı bulunamadı.";

            bool arkadasMi = await _context.Arkadasliklar.AnyAsync(a =>
                (a.GonderenId == gonderenId && a.AlanId == dto.AlanId ||
                 a.GonderenId == dto.AlanId && a.AlanId == gonderenId)
                && a.Durum == "Kabul");

            if (!arkadasMi)
                return "Sadece arkadaşlarınıza mesaj gönderebilirsiniz.";

            Mesaj yeniMesaj = new Mesaj();
            yeniMesaj.GonderenId = gonderenId;
            yeniMesaj.AlanId = dto.AlanId;
            yeniMesaj.Icerik = dto.Icerik;
            yeniMesaj.Tarih = DateTime.Now;
            yeniMesaj.OkunduMu = false;

            _context.Mesajlar.Add(yeniMesaj);
            await _context.SaveChangesAsync();

            return "Mesaj gönderildi.";
        }

        // Konuşmayı görüntüle
        public async Task<List<object>> KonusmayiGor(int kullaniciId, int karsiTarafId)
        {
            // Mesajları okundu yap
            var okunmamisMesajlar = await _context.Mesajlar
                .Where(m => m.GonderenId == karsiTarafId && m.AlanId == kullaniciId && m.OkunduMu == false)
                .ToListAsync();

            foreach (var mesaj in okunmamisMesajlar)
            {
                mesaj.OkunduMu = true;
            }

            await _context.SaveChangesAsync();

            // Konuşmayı getir
            var mesajlar = await _context.Mesajlar
                .Where(m => (m.GonderenId == kullaniciId && m.AlanId == karsiTarafId) ||
                            (m.GonderenId == karsiTarafId && m.AlanId == kullaniciId))
                .OrderBy(m => m.Tarih)
                .Select(m => new
                {
                    m.Id,
                    m.Icerik,
                    m.Tarih,
                    m.OkunduMu,
                    GonderenAd = m.Gonderen.Ad
                })
                .ToListAsync();

            return mesajlar.Cast<object>().ToList();
        }

        // Okunmamış mesaj sayısı
        public async Task<int> OkunmamisMesajSayisi(int kullaniciId)
        {
            int sayi = await _context.Mesajlar
                .CountAsync(m => m.AlanId == kullaniciId && m.OkunduMu == false);

            return sayi;
        }
    }
}