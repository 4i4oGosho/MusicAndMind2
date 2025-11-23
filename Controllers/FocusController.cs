using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAndMind2.Data;
using MusicAndMind2.Models;

namespace MusicAndMind2.Controllers
{
    [Authorize]
    public class FocusController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public FocusController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // 🧩 дефиниции на честотите – тук са ключовете, имената и пътищата
        private static readonly List<FocusTrackDefinition> Tracks = new()
        {
            new("alpha", "Alpha Flow (10 Hz AM)",
                "Спокоен фокус — идеален за учене и четене.",
                "Спокоен", "/images/wave-thumb.png", "/audio/alpha_am_200_10.wav"),

            new("gamma", "Gamma Spark (40 Hz AM)",
                "Дълбока работа — логика, анализ, памет.",
                "Deep Work", "/images/wave-gamma.png", "/audio/gamma_am_400_40.wav"),

            new("hz528", "528 Hz Tone",
                "Емоционален баланс — спокойствие и дишане.",
                "Баланс", "/images/wave-528.jpg", "/audio/528hz.wav"),

            new("hz432", "A=432 Hz Tone",
                "Органична хармония — идеална за спокойни задачи.",
                "Хармония", "/images/wave-432.jpg", "/audio/432hz.wav"),

            new("hz852", "852 Hz Tone",
                "Интуиция и вътрешна яснота — възстановява връзката със собственото Аз.",
                "Интуиция", "/images/wave-852hz.jpg", "/audio/852hz.mp3"),

            new("hz963", "963 Hz Tone",
                "Съзнателност и хармония — честота на „божествената връзка“ и вътрешния мир.",
                "Просветление", "/images/wave-963.jpg", "/audio/963hz.mp3")
        };

        // 🔐 помощ: взимаме дефиниция по ключ
        private FocusTrackDefinition? GetTrack(string key) =>
            Tracks.FirstOrDefault(t => t.Key == key);

        // 💾 Запазване на честота в профила
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string trackKey)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var track = GetTrack(trackKey);
            if (track == null) return NotFound();

            bool exists = await _db.UserFocusTracks
                .AnyAsync(x => x.UserId == userId && x.TrackKey == trackKey);

            if (!exists)
            {
                _db.UserFocusTracks.Add(new UserFocusTrack
                {
                    UserId = userId,
                    TrackKey = trackKey
                });
                await _db.SaveChangesAsync();
            }

            TempData["FocusMessage"] = "Честотата беше запазена в твоя профил.";
            return RedirectToAction("Focus", "Home");
        }

        // 📂 Страница "Моите честоти"
        [HttpGet]
        public async Task<IActionResult> MyTracks()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var userTracks = await _db.UserFocusTracks
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var model = userTracks
                .Select(ut => GetTrack(ut.TrackKey))
                .Where(t => t != null)!
                .ToList();

            return View(model);
        }
    }

    // Малък helper клас за дефиниция на честота
    public class FocusTrackDefinition
    {
        public FocusTrackDefinition(string key, string title, string description,
            string tag, string imageUrl, string audioUrl)
        {
            Key = key;
            Title = title;
            Description = description;
            Tag = tag;
            ImageUrl = imageUrl;
            AudioUrl = audioUrl;
        }

        public string Key { get; }
        public string Title { get; }
        public string Description { get; }
        public string Tag { get; }
        public string ImageUrl { get; }
        public string AudioUrl { get; }
    }
}
