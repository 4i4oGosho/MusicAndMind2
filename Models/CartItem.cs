using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicAndMind2.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // 🔐 Кой потребител притежава този елемент
        [Required]
        public string UserId { get; set; } = string.Empty;

        // 🛒 Кой продукт е добавен
        [Required]
        public int ProductId { get; set; }

        // 🔢 Количество (минимум 1)
        [Range(1, 999, ErrorMessage = "Количество трябва да бъде поне 1.")]
        public int Quantity { get; set; } = 1;

        // 🔗 Навигационно свойство
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
