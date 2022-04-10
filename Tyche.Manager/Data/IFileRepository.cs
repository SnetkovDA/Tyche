using System.Collections.Generic;
using Tyche.Manager.Models;
using Tyche.Shared.Models;

namespace Tyche.Manager.Data
{
    public interface IFileRepository
    {
        public IEnumerable<string> GetAllScannersIds();
        public IEnumerable<string> GetAllFoundFilesIdsForScanner(string scannerId);
        public IEnumerable<string> GetAllMatchesIdsForFile(string fileId);

        public void AddOrUpdateScanner(Scanner scanner);
        public Scanner GetScanner(string id);
        public void DeleteScanner(string id);

        public string AddOrUpdateScanSettings(string scannerId, ScanSettings scanSettings);
        public ScanSettings GetScanSettings(string scannerId);

        public void AddTaskIdForScanner(string scannerId, string taskId);
        public void DeleteTaskIdForScanner(string scannerId);
        public string GetTaskIdForScanner(string scannerId);

        public string GetFileIdByPathToFile(string scannerId, string pathToFile);
        public FoundFile GetFoundFile(string fileId);
        public void AddOrUpdateFoundFile(FoundFile foundFile);
        public void DeleteFoundFile(string fileId);
        
        public void AddOrUpdateMatchInFile(string fileId, MatchInFile match);
        public MatchInFile GetMatchInFile(string fileId, string matchId);
        public void DeleteMatchInFile(string fileId, string matchId);
        public void DeleteAllMatchesInFile(string fileId);
    }
}
