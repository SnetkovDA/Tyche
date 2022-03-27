using System.Collections.Generic;

namespace Tyche.Manager.Models
{
    public class Scanner
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Host { get; set; }
        public string SettingsId { get; set; }
    }
}
