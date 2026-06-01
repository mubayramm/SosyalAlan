using Microsoft.EntityFrameworkCore;
using SosyalAlan.Data;
using SosyalAlan.DTOs;
using SosyalAlan.Models;

namespace SosyalAlan.Services
{
    public class KullaniciService
    {
        private readonly AppDbContext _context;

        public KullaniciService(AppDbContext context)
        {
            _context = context;
        }

        // Kullanıcıları listele ve ara
        public async Task<List<object>> Listele(string? ara)
        {
            var kullanicilar = _context.Kullanicilar.AsQueryable();

            if (!string.IsNullOrEmpty(ara))
            {
                kullanicilar = kullanicilar.Where(k => k.Ad.Contains(ara));
            }

            var sonuc = await kullanicilar.Select(k => new
            {
                k.Id,
                k.Ad,
                k.Eposta,
                k.KayitTarihi
            }).ToListAsync();

            return sonuc.Cast<object>().ToList();
        }

        // Profil görüntüle
        public async Task<object> ProfilGetir(int kullaniciId)
        {
            Kullanici kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
                return null;

            return new
            {
                kullanici.Id,
                kullanici.Ad,
                kullanici.Eposta,
                kullanici.KayitTarihi
            };
        }

        // Profil güncelle
        public async Task<string> ProfilGuncelle(int kullaniciId, ProfilGuncelleDto dto)
        {
            Kullanici kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
                return null;

            if (kullanici.Eposta != dto.Eposta)
            {
                bool epostaVarMi = await _context.Kullanicilar.AnyAsync(k => k.Eposta == dto.Eposta);
                if (epostaVarMi)
                    return "Bu eposta zaten kullanılıyor.";
            }

            kullanici.Ad = dto.Ad;
            kullanici.Eposta = dto.Eposta;

            await _context.SaveChangesAsync();
            return "Profil güncellendi.";
        }

        // Şifre değiştir
        public async Task<string> SifreDegistir(int kullaniciId, SifreDegistirDto dto)
        {
            Kullanici kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
                return null;

            bool eskiSifreDogruMu = BCrypt.Net.BCrypt.Verify(dto.EskiSifre, kullanici.SifreHash);
            if (!eskiSifreDogruMu)
                return "Eski şifre hatalı.";

            kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
            await _context.SaveChangesAsync();

            return "Şifre değiştirildi.";
        }
    }
}