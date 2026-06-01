using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SosyalAlan.Data;
using SosyalAlan.DTOs;
using SosyalAlan.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SosyalAlan.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Kayit(KayitDto dto)
        {
            bool epostaVarMi = await _context.Kullanicilar.AnyAsync(k => k.Eposta == dto.Eposta);
            if (epostaVarMi)
                return null;

            Kullanici yeniKullanici = new Kullanici();
            yeniKullanici.Ad = dto.Ad;
            yeniKullanici.Eposta = dto.Eposta;
            yeniKullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);
            yeniKullanici.KayitTarihi = DateTime.Now;

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            return "Kayıt başarılı.";
        }

        public async Task<string> Giris(GirisDto dto)
        {
            Kullanici kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta == dto.Eposta);
            if (kullanici == null)
                return null;

            bool sifreDogruMu = BCrypt.Net.BCrypt.Verify(dto.Sifre, kullanici.SifreHash);
            if (!sifreDogruMu)
                return null;

            return TokenUret(kullanici);
        }

        private string TokenUret(Kullanici kullanici)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("id", kullanici.Id.ToString()));
            claims.Add(new Claim("ad", kullanici.Ad));

            string gizliAnahtar = _configuration["Jwt:Key"];
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(gizliAnahtar));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}