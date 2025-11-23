using System.ComponentModel.DataAnnotations;

namespace MusicAndMind2.Models
{
    public class UserFocusTrack
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Ключ на честотата (напр. "alpha", "gamma", "hz528"…)
        [Required]
        public string TrackKey { get; set; } = string.Empty;
    }
}
