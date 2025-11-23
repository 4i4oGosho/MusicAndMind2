using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicAndMind2.Models;

namespace MusicAndMind2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 🧠 Запазени честоти за фокус по потребител
        public DbSet<UserFocusTrack> UserFocusTracks { get; set; } = null!;
    }
}

