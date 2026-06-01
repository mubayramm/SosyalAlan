using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SosyalAlan.DTOs;
using SosyalAlan.Services;

namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MesajController : ControllerBase
    {
        private readonly MesajService _mesajService;

        public MesajController(MesajService mesajService)
        {
            _mesajService = mesajService;
        }

        // POST api/mesaj
        [HttpPost]
        public async Task<IActionResult> MesajGonder(MesajGonderDto dto)
        {
            int gonderenId = int.Parse(User.FindFirst("id").Value);
            string sonuc = await _mesajService.MesajGonder(gonderenId, dto);

            if (sonuc == "Alıcı bulunamadı.")
                return NotFound(sonuc);

            if (sonuc != "Mesaj gönderildi.")
                return BadRequest(sonuc);

            return Ok(sonuc);
        }

        // GET api/mesaj/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> KonusmayiGor(int id)
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            var sonuc = await _mesajService.KonusmayiGor(kullaniciId, id);
            return Ok(sonuc);
        }

        // GET api/mesaj/okunmamis
        [HttpGet("okunmamis")]
        public async Task<IActionResult> OkunmamisMesajSayisi()
        {
            int kullaniciId = int.Parse(User.FindFirst("id").Value);
            int sayi = await _mesajService.OkunmamisMesajSayisi(kullaniciId);
            return Ok(new { okunmamisSayi = sayi });
        }
    }
}