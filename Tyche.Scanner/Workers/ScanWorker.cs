using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Tyche.Scanner.Models;
using Tyche.Shared.Models;

namespace Tyche.Scanner.Workers
{
    public class ScanWorker
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly WebWorker _webWorker;
        private readonly ScanSettings _scanSettings;

        public ScanWorker(SettingsProvider settingsProvider, WebWorker webWorker, ScanSettings scanSettings)
        {
            _settingsProvider = settingsProvider;
            _webWorker = webWorker;
            _scanSettings = scanSettings;
        }

        public FileInfo[] GetFiles(bool includeSubfolders)
        {
            Settings settings = _settingsProvider.GetSettings();
            return new DirectoryInfo(settings.ScanPath)
                .GetFiles(settings.ScanMask, includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(f => f.CreationTimeUtc > settings.LastScanDate)
                .ToArray();
        }
         

        public void ScanDirectory()
        {
            var settings = _settingsProvider.GetSettings();
            var files = GetFiles(_scanSettings.IncludeSubfolders);
            foreach (var file in files)
            {
                _webWorker.SendRequest(System.Net.Http.HttpMethod.Put, "File/AddContent", ReadFileContent(file.FullName));
            }
            settings.LastScanDate = System.DateTime.UtcNow;
            _settingsProvider.UpdateSettings(settings);
        }

        private string ReadFileContent(string pathToFile)
        {
            FileScanner scanner = new(pathToFile);
            return JsonConvert.SerializeObject(scanner.ScanFile(_scanSettings.DesiredFields, _scanSettings.ScannerId));
        }
    }
}
