using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SosyalAlan.Data;
using SosyalAlan.DTOs;
using SosyalAlan.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SosyalAlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("kayit")]
        public async Task<IActionResult> Kayit(KayitDto dto)
        {
            // Bu eposta daha önce kullanılmışmı kontrol edelim
            bool epostaVarMi = await _context.Kullanicilar.AnyAsync(k => k.Eposta == dto.Eposta);
            if (epostaVarMi)
                return BadRequest("Bu eposta zaten kayıtlı.");

            // Yeni kullanıcı oluştur
            Kullanici yeniKullanici = new Kullanici();
            yeniKullanici.Ad = dto.Ad;
            yeniKullanici.Eposta = dto.Eposta;
            yeniKullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);
            yeniKullanici.KayitTarihi = DateTime.Now;

            
            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı.");
        }

        [HttpPost("giris")]
        public async Task<IActionResult> Giris(GirisDto dto)
        {
            // Kullanıcı varmı kontrolü
            Kullanici kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta == dto.Eposta);
            if (kullanici == null)
                return BadRequest("Eposta veya şifre hatalı.");

            // şifre doğrumu 
            bool sifreDogruMu = BCrypt.Net.BCrypt.Verify(dto.Sifre, kullanici.SifreHash);
            if (!sifreDogruMu)
                return BadRequest("Eposta veya şifre hatalı.");

            // Token üret
            string token = TokenUret(kullanici);
            return Ok(new { token });
        }

        private string TokenUret(Kullanici kullanici)
        {
            // Token içine kullanıcı bilgilerini yaz
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("id", kullanici.Id.ToString()));
            claims.Add(new Claim("ad", kullanici.Ad));

            // Gizli anahtarı al
            string gizliAnahtar = _configuration["Jwt:Key"];
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(gizliAnahtar));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tokenı oluştur
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            // Tokenı stringe çevir ve returnle (döndür)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}