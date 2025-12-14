namespace MusicAndMind2.Models
{
    public class CartSessionItem
    {
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set; } = 1;
    }
}
