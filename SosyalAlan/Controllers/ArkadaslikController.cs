using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SosyalAlan.DTOs;
using SosyalAlan.Services;

namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ArkadaslikController : ControllerBase
    {
        private readonly ArkadaslikService _arkadaslikService;

        public ArkadaslikController(ArkadaslikService arkadaslikService)
        {
            _arkadaslikService = arkadaslikService;
        }

        // POST api/arkadaslik
        [HttpPost]
        public async Task<IActionResult> IstekGonder(ArkadaslikIstekDto dto)
        {
            int gonderenId = int.Parse(User.FindFirst("id").Value);
            string sonuc = await _arkadaslikService.IstekGonder(gonderenId, dto);

            if (sonuc == "Kullanıcı bulunamadı.")
                return NotFound(sonuc);

            if (sonuc != "Arkadaşlık isteği gönderildi.")
                return BadRequest(sonuc);

            return Ok(sonuc);
        }

        // PATCH api/arkadaslik/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> IstekYanıtla(int id, ArkadaslikYanitDto dto)
        {
            int alanId = int.Parse(User.FindFirst("id").Value);
            string sonuc = await _arkadaslikService.IstekYanıtla(alanId, id, dto);

            if (sonuc == null)
                return NotFound("İstek bulunamadı.");

            if (sonuc == "Bu istek zaten yanıtlanmış.")
                return BadRequest(sonuc);

            return Ok(sonuc);
        }

        // GET api/arkadaslik
        [HttpGet]
        public async Task<IActionResult> ArkadasListesi()
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            var sonuc = await _arkadaslikService.ArkadasListesi(kullaniciId);
            return Ok(sonuc);
        }

        // GET api/arkadaslik/bekleyen
        [HttpGet("bekleyen")]
        public async Task<IActionResult> BekleyenIstekler()
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            var sonuc = await _arkadaslikService.BekleyenIstekler(kullaniciId);
            return Ok(sonuc);
        }

        // DELETE api/arkadaslik/{arkadasId}
        [HttpDelete("{arkadasId}")]
        public async Task<IActionResult> ArkadaslikSil(int arkadasId)
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            string sonuc = await _arkadaslikService.ArkadaslikSil(kullaniciId, arkadasId);

            if (sonuc == null)
                return NotFound("Arkadaşlık bulunamadı.");

            return Ok(sonuc);
        }
    }
}