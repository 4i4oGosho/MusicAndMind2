using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicAndMind2.Models;

namespace MusicAndMind2.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 🛒 Продукти в магазина
        public DbSet<Product> Products { get; set; } = null!;

        // 🛒 Елементи в количката
        public DbSet<CartItem> CartItems { get; set; } = null!;

        // 🎵 Дефинирани честоти (ако имаш FrequencyTrack.cs)
        public DbSet<FrequencyTrack> FrequencyTracks { get; set; } = null!;

        // 👤 Запазени честоти от потребителя
        public DbSet<UserFocusTrack> UserFocusTracks { get; set; } = null!;
    }
}
