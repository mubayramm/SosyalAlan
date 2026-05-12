using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SosyalAlan.Data;

namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KullaniciController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KullaniciController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/kullanicilariara
        [HttpGet]
        public async Task<IActionResult> Listele(string? ara)
        {
            // tüm kullanıcıları al
            var kullanicilar = _context.Kullanicilar.AsQueryable();

            // Arama yaptıysak filtrele
            if (!string.IsNullOrEmpty(ara))
            {
                kullanicilar = kullanicilar.Where(k => k.Ad.Contains(ara));
            }

            // Sadece gerekli alanları döndür assas bilgileri gönderme
            var sonuc = await kullanicilar.Select(k => new
            {
                k.Id,
                k.Ad,
                k.Eposta,
                k.KayitTarihi
            }).ToListAsync();

            return Ok(sonuc);
        }
    }
}