using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SosyalAlan.Data;
using SosyalAlan.DTOs;
using SosyalAlan.Models;

namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MesajController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MesajController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/mesaj
        [HttpPost]
        public async Task<IActionResult> MesajGonder(MesajGonderDto dto)
        {
            
            int gonderenId = int.Parse(User.FindFirst("id").Value);

            // Alıcı var mı?
            bool aliciVarMi = await _context.Kullanicilar.AnyAsync(k => k.Id == dto.AlanId);
            if (!aliciVarMi)
                return NotFound("Alıcı bulunamadı.");

            // Kendine mesaj göndermeye çalışıyor mu?
            if (gonderenId == dto.AlanId)
                return BadRequest("Kendinize mesaj gönderemezsiniz.");

            // Arkadaş mı kontrol et
            bool arkadasMi = await _context.Arkadasliklar.AnyAsync(a =>
                (a.GonderenId == gonderenId && a.AlanId == dto.AlanId ||
                 a.GonderenId == dto.AlanId && a.AlanId == gonderenId)
                && a.Durum == "Kabul");

            if (!arkadasMi)
                return BadRequest("Sadece arkadaşlarınıza mesaj gönderebilirsiniz.");

            // Yeni mesaj oluştur
            Mesaj yeniMesaj = new Mesaj();
            yeniMesaj.GonderenId = gonderenId;
            yeniMesaj.AlanId = dto.AlanId;
            yeniMesaj.Icerik = dto.Icerik;
            yeniMesaj.Tarih = DateTime.Now;
            yeniMesaj.OkunduMu = false;

            _context.Mesajlar.Add(yeniMesaj);
            await _context.SaveChangesAsync();

            return Ok("Mesaj gönderildi.");
        }

        // GET api/mesaj/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> KonusmayiGor(int id)
        {
            // Token'dan giriş yapan kullanıcının idsini al 
            int kullaniciId = int.Parse(User.FindFirst("id").Value);

            // İki kullanıcının birbiriyle mesajlarını getir
            var mesajlar = await _context.Mesajlar
                .Where(m => (m.GonderenId == kullaniciId && m.AlanId == id) ||
                            (m.GonderenId == id && m.AlanId == kullaniciId))
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

            return Ok(mesajlar);
        }
    }
}