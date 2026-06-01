using Microsoft.AspNetCore.Mvc;
using SosyalAlan.DTOs;
using SosyalAlan.Services;

namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("kayit")]
        public async Task<IActionResult> Kayit(KayitDto dto)
        {
            string sonuc = await _authService.Kayit(dto);
            if (sonuc == null)
                return BadRequest("Bu eposta zaten kayıtlı.");

            return Ok(sonuc);
        }

        [HttpPost("giris")]
        public async Task<IActionResult> Giris(GirisDto dto)
        {
            string token = await _authService.Giris(dto);
            if (token == null)
                return BadRequest("Eposta veya şifre hatalı.");

            return Ok(new { token });
        }
    }
}