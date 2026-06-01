using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SosyalAlan.DTOs;
using SosyalAlan.Services;

namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KullaniciController : ControllerBase
    {
        private readonly KullaniciService _kullaniciService;

        public KullaniciController(KullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        // GET api/kullanici
        [HttpGet]
        public async Task<IActionResult> Listele(string? ara)
        {
            var sonuc = await _kullaniciService.Listele(ara);
            return Ok(sonuc);
        }

        // GET api/kullanici/profil
        [HttpGet("profil")]
        public async Task<IActionResult> ProfilGetir()
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            var sonuc = await _kullaniciService.ProfilGetir(kullaniciId);
            if (sonuc == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(sonuc);
        }

        // PUT api/kullanici/profil
        [HttpPut("profil")]
        public async Task<IActionResult> ProfilGuncelle(ProfilGuncelleDto dto)
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            string sonuc = await _kullaniciService.ProfilGuncelle(kullaniciId, dto);

            if (sonuc == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (sonuc == "Bu eposta zaten kullanılıyor.")
                return BadRequest(sonuc);

            return Ok(sonuc);
        }

        // PATCH api/kullanici/sifre
        [HttpPatch("sifre")]
        public async Task<IActionResult> SifreDegistir(SifreDegistirDto dto)
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            string sonuc = await _kullaniciService.SifreDegistir(kullaniciId, dto);

            if (sonuc == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (sonuc == "Eski şifre hatalı.")
                return BadRequest(sonuc);

            return Ok(sonuc);
        }
    }
}