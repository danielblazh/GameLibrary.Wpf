namespace GameLibrary.Wpf.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-10
        public string Status { get; set; } = "Playing"; // Playing / Queued / Completed / Dropped
        public double HoursPlayed { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
