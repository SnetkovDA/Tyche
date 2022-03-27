namespace Tyche.Shared.Models
{
    public class ScanSettings
    {
        public string ScannerId { get; set; }
        public DesiredField[] DesiredFields { get; set; }
        public bool IncludeSubfolders { get; set; }
        public bool UsePreviousScanDate { get; set; }
    }
}
