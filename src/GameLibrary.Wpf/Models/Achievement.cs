namespace GameLibrary.Wpf.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
