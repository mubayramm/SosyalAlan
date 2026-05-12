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
    public class ArkadaslikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArkadaslikController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/arkadaslik
        [HttpPost]
        public async Task<IActionResult> IstekGonder(ArkadaslikIstekDto dto)
        {
            // Token'dan giriş yapan kullanıcının Id'sini al
            int gonderenId = int.Parse(User.FindFirst("id").Value);

            // Kendine istek göndermeye çalışıyor mu?
            if (gonderenId == dto.AlanId)
                return BadRequest("Kendinize arkadaşlık isteği gönderemezsiniz.");

            // Daha önce istek gönderilmiş mi?
            bool istekVarMi = await _context.Arkadasliklar.AnyAsync(a =>
                a.GonderenId == gonderenId && a.AlanId == dto.AlanId);

            if (istekVarMi)
                return BadRequest("Zaten bir isteğiniz var.");

            
            Arkadaslik yeniIstek = new Arkadaslik();
            yeniIstek.GonderenId = gonderenId;
            yeniIstek.AlanId = dto.AlanId;
            yeniIstek.Durum = "Beklemede";
            yeniIstek.GonderimTarihi = DateTime.Now;

            _context.Arkadasliklar.Add(yeniIstek);
            await _context.SaveChangesAsync();

            return Ok("Arkadaşlık isteği gönderildi.");
        }

        
        [HttpPatch("{id}")]
        public async Task<IActionResult> IstekYanıtla(int id, ArkadaslikYanitDto dto)
        {
            // Token'dan giriş yapan kullanıcının Id'sini al
            int alanId = int.Parse(User.FindFirst("id").Value);

            // İsteği bul
            Arkadaslik istek = await _context.Arkadasliklar.FirstOrDefaultAsync(a =>
                a.Id == id && a.AlanId == alanId);

            if (istek == null)
                return NotFound("İstek bulunamadı.");

            // Zaten yanıtlanmış mı?
            if (istek.Durum != "Beklemede")
                return BadRequest("Bu istek zaten yanıtlanmış.");

            
            istek.Durum = dto.Durum; 
            await _context.SaveChangesAsync();

            return Ok("İstek yanıtlandı.");
        }

        // GET api/arkadaslik
        [HttpGet]
        public async Task<IActionResult> ArkadasListesi()
        {
            // Token'dan giriş yapan kullanıcının Id'sini al
            int kullaniciId = int.Parse(User.FindFirst("id").Value);

            // Kabul edilmiş arkadaşlıkları getir
            var arkadaslar = await _context.Arkadasliklar
                .Where(a => (a.GonderenId == kullaniciId || a.AlanId == kullaniciId)
                    && a.Durum == "Kabul")
                .Select(a => new
                {
                    ArkadasId = a.GonderenId == kullaniciId ? a.AlanId : a.GonderenId,
                    ArkadasAd = a.GonderenId == kullaniciId ? a.Alan.Ad : a.Gonderen.Ad
                })
                .ToListAsync();

            return Ok(arkadaslar);
        }
    }
}