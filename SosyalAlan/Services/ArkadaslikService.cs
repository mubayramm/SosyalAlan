using Microsoft.EntityFrameworkCore;
using SosyalAlan.Data;
using SosyalAlan.DTOs;
using SosyalAlan.Models;

namespace SosyalAlan.Services
{
    public class ArkadaslikService
    {
        private readonly AppDbContext _context;

        public ArkadaslikService(AppDbContext context)
        {
            _context = context;
        }

        // Arkadaşlık isteği gönder
        public async Task<string> IstekGonder(int gonderenId, ArkadaslikIstekDto dto)
        {
            if (gonderenId == dto.AlanId)
                return "Kendinize arkadaşlık isteği gönderemezsiniz.";

            bool aliciVarMi = await _context.Kullanicilar.AnyAsync(k => k.Id == dto.AlanId);
            if (!aliciVarMi)
                return "Kullanıcı bulunamadı.";

            bool istekVarMi = await _context.Arkadasliklar.AnyAsync(a =>
                a.GonderenId == gonderenId && a.AlanId == dto.AlanId);
            if (istekVarMi)
                return "Zaten bir isteğiniz var.";

            Arkadaslik yeniIstek = new Arkadaslik();
            yeniIstek.GonderenId = gonderenId;
            yeniIstek.AlanId = dto.AlanId;
            yeniIstek.Durum = "Beklemede";
            yeniIstek.GonderimTarihi = DateTime.Now;

            _context.Arkadasliklar.Add(yeniIstek);
            await _context.SaveChangesAsync();

            return "Arkadaşlık isteği gönderildi.";
        }

        // İsteği kabul et veya reddet
        public async Task<string> IstekYanıtla(int alanId, int istekId, ArkadaslikYanitDto dto)
        {
            Arkadaslik istek = await _context.Arkadasliklar.FirstOrDefaultAsync(a =>
                a.Id == istekId && a.AlanId == alanId);
            if (istek == null)
                return null;

            if (istek.Durum != "Beklemede")
                return "Bu istek zaten yanıtlanmış.";

            istek.Durum = dto.Durum;
            await _context.SaveChangesAsync();

            return "İstek yanıtlandı.";
        }

        // Arkadaş listesi
        public async Task<List<object>> ArkadasListesi(int kullaniciId)
        {
            var arkadaslar = await _context.Arkadasliklar
                .Where(a => (a.GonderenId == kullaniciId || a.AlanId == kullaniciId)
                    && a.Durum == "Kabul")
                .Select(a => new
                {
                    ArkadasId = a.GonderenId == kullaniciId ? a.AlanId : a.GonderenId,
                    ArkadasAd = a.GonderenId == kullaniciId ? a.Alan.Ad : a.Gonderen.Ad
                })
                .ToListAsync();

            return arkadaslar.Cast<object>().ToList();
        }

        // Bekleyen istekler
        public async Task<List<object>> BekleyenIstekler(int kullaniciId)
        {
            var istekler = await _context.Arkadasliklar
                .Where(a => a.AlanId == kullaniciId && a.Durum == "Beklemede")
                .Select(a => new
                {
                    a.Id,
                    GonderenAd = a.Gonderen.Ad,
                    a.GonderimTarihi
                })
                .ToListAsync();

            return istekler.Cast<object>().ToList();
        }

        // Arkadaşlıktan çıkar
        public async Task<string> ArkadaslikSil(int kullaniciId, int arkadasId)
        {
            Arkadaslik arkadaslik = await _context.Arkadasliklar.FirstOrDefaultAsync(a =>
                (a.GonderenId == kullaniciId && a.AlanId == arkadasId ||
                 a.GonderenId == arkadasId && a.AlanId == kullaniciId)
                && a.Durum == "Kabul");

            if (arkadaslik == null)
                return null;

            _context.Arkadasliklar.Remove(arkadaslik);
            await _context.SaveChangesAsync();

            return "Arkadaşlıktan çıkarıldı.";
        }
    }
}