namespace Tyche.Manager.Models
{
    public class FoundFile
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public string ScannerId { get; set; }
        public int MatchesCount { get; set; } = 0;
        public double Timestamp { get; set; } = System.DateTime.UtcNow.ToOADate();
    }
}
