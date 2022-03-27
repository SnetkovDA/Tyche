using Microsoft.AspNetCore.Mvc;
using Tyche.Manager.Data;
using Tyche.Manager.Models;
using Tyche.Shared.Models;

namespace Tyche.Manager.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileController : Controller
    {
        readonly IFileRepository _fileRepository;

        public FileController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpGet]
        [Route("Ids/{id}")]
        public IActionResult GetFileIdsForScanner([FromRoute] string id)
        {
            return new JsonResult(_fileRepository.GetAllFoundFilesIdsForScanner(id));
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult GetFileById([FromRoute] string id)
        {
            return new JsonResult(_fileRepository.GetFoundFile(id));
        }

        [HttpPut]
        [Route("Add")]
        public IActionResult AddFileContent([FromForm] FoundFileMatches fileContent)
        {
            string id = _fileRepository.GetFileIdByPathToFile(fileContent.ScannerId, fileContent.FileName);
            FoundFile file = new()
            {
                FileName = fileContent.FileName,
                ScannerId = fileContent.ScannerId
            };
            if (!string.IsNullOrEmpty(id))
            {
                file.ScannerId = id;
                _fileRepository.DeleteAllMatchesInFile(file.Id);
            }
            file.Timestamp = System.DateTime.UtcNow.ToOADate();
            foreach (var match in fileContent.FoundMatches)
            {
                _fileRepository.AddOrUpdateMatchInFile(file.Id, new MatchInFile
                {
                    Name = match.Name,
                    Count = match.MatchesCount,
                    Content = (match.Matches != null && match.Matches.Length > 0) ? match.Matches : System.Array.Empty<string>()
                });
                file.MatchesCount += match.MatchesCount;
            }
            _fileRepository.AddOrUpdateFoundFile(file);
            return Ok();
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeleteFile([FromRoute] string id)
        {
            _fileRepository.DeleteFoundFile(id);
            return NoContent();
        }

    }
}
