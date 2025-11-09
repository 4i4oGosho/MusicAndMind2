using System.ComponentModel.DataAnnotations;

namespace MusicAndMind2.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 9999, ErrorMessage = "Цената трябва да е между 0 и 9999 лв.")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }   // 🖼 снимка на продукта
        public string? SoundUrl { get; set; }   // 🎵 звук за прослушване
        public string? FrequencyInfo { get; set; } // 🧠 информация за мозъчните честоти

        // 🧩 нови свойства за разширяване:
        public string? Category { get; set; }   // напр. "Купи", "Камертон", "Гонг"
        public bool IsAvailable { get; set; } = true; // наличност
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 💳 избор на начин на плащане (само за визуализиране)
        public string? PaymentMethod { get; set; } // "Карта" или "Наложен платеж"
    }
}
