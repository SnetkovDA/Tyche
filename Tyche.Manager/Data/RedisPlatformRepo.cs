using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Tyche.Manager.Models;
using Tyche.Shared.Models;

namespace Tyche.Manager.Data
{
    public class RedisPlatformRepo : IFileRepository
    {
        private const string ScannersSetName = "Scanners";
        private const string ScanSettingsPrefix = "ScanSettings_";
        private const string TaskScanPrefix = "ScanTask_";
        private const string FilesPrefixSetName = "Files_";
        private const string MatchPrefixSetName = "Match_";

        readonly IConnectionMultiplexer _redis;

        public RedisPlatformRepo(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        
        public IEnumerable<string> GetAllScannersIds()
        {
            var db = _redis.GetDatabase();
            return db.SetMembers(ScannersSetName).Select(s => s.ToString());
        }
        
        public void AddOrUpdateScanner(Scanner scanner)
        {
            var db = _redis.GetDatabase();
            db.StringSet(scanner.Id, JsonSerializer.Serialize(scanner));
            db.SetAdd(ScannersSetName, scanner.Id);
        }

        public Scanner GetScanner(string id)
        {
            var db = _redis.GetDatabase();
            return JsonSerializer.Deserialize<Scanner>(db.StringGet(id));
        }

        public void DeleteScanner(string id)
        {
            var db = _redis.GetDatabase();
            db.KeyDelete(id);
            db.KeyDelete(ScanSettingsPrefix + id);
            db.KeyDelete(TaskScanPrefix + id);
            foreach (var fileId in GetAllFoundFilesIdsForScanner(id))
                DeleteFoundFile(fileId);
            db.SetRemove(ScannersSetName, id);
        }

        public void AddOrUpdateScanSettings(string scannerId, ScanSettings scanSettings)
        {
            var db = _redis.GetDatabase();
            db.StringSet(ScanSettingsPrefix + scannerId, JsonSerializer.Serialize(scanSettings));
        }

        public ScanSettings GetScanSettings(string scannerId)
        {
            var db = _redis.GetDatabase();
            return JsonSerializer.Deserialize<ScanSettings>(db.StringGet(ScanSettingsPrefix + scannerId));
        }

        public void AddTaskIdForScanner(string scannerId, string taskId)
        {
            var db = _redis.GetDatabase();
            db.StringSet(TaskScanPrefix + scannerId, taskId);
        }

        public string GetTaskIdForScanner(string scannerId)
        {
            var db = _redis.GetDatabase();
            return db.StringGet(TaskScanPrefix + scannerId);
        }

        public void DeleteTaskIdForScanner(string scannerId)
        {
            var db = _redis.GetDatabase();
            db.KeyDelete(TaskScanPrefix + scannerId);
        }

        public IEnumerable<string> GetAllFoundFilesIdsForScanner(string scannerId)
        {
            var db = _redis.GetDatabase();
            return db.SetMembers(FilesPrefixSetName + scannerId).Select(s => s.ToString());
        }
        
        public void AddOrUpdateFoundFile(FoundFile foundFile)
        {
            var db = _redis.GetDatabase();
            db.SetAdd(FilesPrefixSetName + foundFile.ScannerId, foundFile.Id);
            db.StringSet(foundFile.Id, JsonSerializer.Serialize(foundFile));
        }

        public string GetFileIdByPathToFile(string scannerId, string pathToFile)
        {
            var ids = GetAllFoundFilesIdsForScanner(scannerId);
            foreach (var id in ids)
            {
                FoundFile foundFile = GetFoundFile(id);
                if (string.Equals(foundFile.FileName, pathToFile))
                    return foundFile.Id;
            }
            return "";
        }

        public FoundFile GetFoundFile(string fileId)
        {
            var db = _redis.GetDatabase();
            return JsonSerializer.Deserialize<FoundFile>(db.StringGet(fileId));
        }

        public void DeleteFoundFile(string fileId)
        {
            var db = _redis.GetDatabase();
            FoundFile foundFile = GetFoundFile(fileId);
            db.KeyDelete(fileId);
            DeleteAllMatchesInFile(fileId);
            db.SetRemove(FilesPrefixSetName + foundFile.ScannerId, fileId);
        }

        public IEnumerable<string> GetAllMatchesIdsForFile(string fileId)
        {
            var db = _redis.GetDatabase();
            return db.SetMembers(MatchPrefixSetName + fileId).Select(s => s.ToString());
        }

        public void AddOrUpdateMatchInFile(string fileId, MatchInFile match)
        {
            var db = _redis.GetDatabase();
            db.SetAdd(MatchPrefixSetName + fileId, match.Id);
            db.StringSet(match.Id, JsonSerializer.Serialize(match));
        }

        public MatchInFile GetMatchInFile(string fileId, string matchId)
        {
            var db = _redis.GetDatabase();
            return JsonSerializer.Deserialize<MatchInFile>(db.StringGet(matchId));
        }

        public void DeleteMatchInFile(string fileId, string matchId)
        {
            var db = _redis.GetDatabase();
            db.KeyDelete(matchId);
            db.SetRemove(MatchPrefixSetName + fileId, matchId);
        }

        public void DeleteAllMatchesInFile(string fileId)
        {
            if (GetFoundFile(fileId) == null)
                return;
            foreach (var matchId in GetAllMatchesIdsForFile(fileId))
                DeleteMatchInFile(matchId, fileId);
        }
    }
}
