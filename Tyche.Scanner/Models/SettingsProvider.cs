using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Tyche.Scanner.Models
{
    public class SettingsProvider
    {
        private readonly string _pathToFile;

        public SettingsProvider(string pathToFile)
        {
            this._pathToFile = pathToFile;
            if (!File.Exists(pathToFile))
                UpdateSettings(GetDefaultSettings());
        }

        public Settings GetSettings()
        {
            using StreamReader reader = new(_pathToFile);
            return JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd());
        }

        public void UpdateSettings(Settings settings)
        {
            File.WriteAllText(_pathToFile, JsonConvert.SerializeObject(settings));
        }

        public Settings GetDefaultSettings() => new()
        {
            LastScanDate = System.DateTime.MinValue,
            OutHostName = "http://localhost:8080",
            ScanMask = "*.*",
            ScanPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        };
    }
}
