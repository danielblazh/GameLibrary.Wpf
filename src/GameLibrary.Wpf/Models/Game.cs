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
        public int Level { get; set; } = 1; // 1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert, 5=Master
        public string Notes { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
