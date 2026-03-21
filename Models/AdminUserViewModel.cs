namespace MusicAndMind2.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public string AdminNote { get; set; } = string.Empty;
    }
}