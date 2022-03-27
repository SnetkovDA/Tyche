using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tyche.Shared.Models;

namespace Tyche.Scanner.Workers
{
    public class FileScanner
    {
        public readonly string fileName;

        public FileScanner(string fileName)
        {
            this.fileName = fileName;
        }

        public FoundFileMatches ScanFile(DesiredField[] desiredFields, string scannerId)
        {
            using var reader = new StreamReader(fileName);
            var content = reader.ReadToEnd();
            FoundFileMatches foundFileMatches = new()
            {
                FileName = fileName,
                ScannerId = scannerId,
                FoundMatches = new FoundMatch[desiredFields.Length]
            };
            for (int i = 0; i < desiredFields.Length; i++)
            {
                var field = desiredFields[i];
                foundFileMatches.FoundMatches[i] = ScanFileContent(content, field);
            }
            return foundFileMatches;
        }

        private FoundMatch ScanFileContent(string fileContent, DesiredField field)
        {
            FoundMatch foundContent = new() { Name = field.Name, Matches = Array.Empty<string>() };
            Regex regex = new(field.Pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            var matches = regex.Matches(fileContent);
            if (matches.Count > 0 && field.IsRegex)
                foundContent.Matches = matches.Select(m => m.Value).ToArray();
            foundContent.MatchesCount = matches.Count;
            return foundContent;
        }
    }
}
