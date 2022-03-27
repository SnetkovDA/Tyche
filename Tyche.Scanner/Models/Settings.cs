using System;

namespace Tyche.Scanner.Models
{
    public class Settings
    {
        public string ScanPath { get; set; }
        public string ScanMask { get; set; }
        public string OutHostName { get; set; }
        public DateTime LastScanDate { get; set; }
        
        public void Merge(Settings settings)
        {
            ScanPath = settings.ScanPath;
            ScanMask = settings.ScanMask;
            OutHostName = settings.OutHostName;
            LastScanDate = settings.LastScanDate;
        }
    }
}
