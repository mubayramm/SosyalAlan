using Microsoft.EntityFrameworkCore;
using SosyalAlan.Models;

namespace SosyalAlan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Arkadaslik> Arkadasliklar { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Arkadaslik tablosu için ilişkiler
            modelBuilder.Entity<Arkadaslik>()
                .HasOne(a => a.Gonderen)
                .WithMany()
                .HasForeignKey(a => a.GonderenId)
                .OnDelete(DeleteBehavior.Restrict);// restrict ; ona bağlı kayıtları sildikten sonra siliyoruz.

            modelBuilder.Entity<Arkadaslik>()
                .HasOne(a => a.Alan)
                .WithMany()
                .HasForeignKey(a => a.AlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mesaj tablosu için ilişkiler
            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Gonderen)
                .WithMany()
                .HasForeignKey(m => m.GonderenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Alan)
                .WithMany()
                .HasForeignKey(m => m.AlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}