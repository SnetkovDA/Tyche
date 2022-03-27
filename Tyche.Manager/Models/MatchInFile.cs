namespace Tyche.Manager.Models
{
    public class MatchInFile
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public int Count { get; set; }
        public string[] Content { get; set; }
    }
}
