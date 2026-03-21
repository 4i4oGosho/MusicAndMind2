using System.ComponentModel.DataAnnotations;

namespace MusicAndMind2.Models
{
    public class AdminNote
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Note { get; set; } = string.Empty;
    }
}