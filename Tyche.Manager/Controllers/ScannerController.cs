using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Tyche.Manager.Data;
using Tyche.Manager.Models;
using Tyche.Shared.Models;

namespace Tyche.Manager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScannerController : Controller
    {
        readonly IFileRepository _fileRepository;

        public ScannerController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpGet("AllScanners")]
        public IActionResult AllScanners()
        {
            IEnumerable<string> ids = _fileRepository.GetAllScannersIds();
            return new JsonResult(ids.Select(id => _fileRepository.GetScanner(id)));
        }

        [HttpGet("{id}")]
        public IActionResult GetScanner(string id)
        {
            return new JsonResult(_fileRepository.GetScanner(id));
        }

        [HttpPut("Add")]
        public IActionResult AddScanner([FromBody] Scanner scanner)
        {
            scanner.SettingsId = _fileRepository.AddOrUpdateScanSettings(scanner.Id, new ScanSettings 
            {
                ScannerId = scanner.Id,
                IncludeSubfolders = true,
                UsePreviousScanDate = false
            });
            _fileRepository.AddOrUpdateScanner(scanner);
            return Ok(new { id = scanner.Id });
        }

        [HttpPost("Update")]
        public IActionResult UpdateScanner([FromBody] Scanner scanner)
        {
            _fileRepository.AddOrUpdateScanner(scanner);
            return Ok(new { id = scanner.Id });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteScanner(string id)
        {
            _fileRepository.DeleteScanner(id);
            return NoContent();
        }

        [HttpGet("ScanSettings/{id}")]
        public IActionResult GetScanSettings(string id)
        {
            return new JsonResult(_fileRepository.GetScanSettings(id));
        }

        [HttpPost("ScanSettings")]
        public IActionResult UpdateScanSettings([FromBody] ScanSettings scanSettings)
        {
            _fileRepository.AddOrUpdateScanSettings(scanSettings.ScannerId, scanSettings);
            return Ok();
        }

        [HttpGet]
        [Route("Status/{id}")]
        public IActionResult GetScannerStatus(string id)
        {
            Scanner scanner = _fileRepository.GetScanner(id);
            using HttpClient httpClient = new();
            try
            {
                HttpResponseMessage response = httpClient.Send(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{scanner.Host}/Scan/Status")
                });
                return Ok(new { status = response.IsSuccessStatusCode ? "Active" : "Error", error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult() });
            }
            catch (Exception e)
            {
                return Ok(new { status = "Exception", error = e.Message });
            }
        }

        [HttpGet]
        [Route("LocalSettings/{id}")]
        public IActionResult GetScannerLocalSettings(string id)
        {
            Scanner scanner = _fileRepository.GetScanner(id);
            using HttpClient httpClient = new();
            try
            {
                HttpResponseMessage response = httpClient.Send(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{scanner.Host}/Scan/Settings")
                });
                return Ok(new {settings = response.Content.ReadAsStringAsync().GetAwaiter().GetResult() });
            }
            catch (Exception e)
            {
                return Ok(new { status = "Exception", error = e.Message });
            }
        }

        [HttpPost]
        [Route("Rescan/{id}")]
        public IActionResult StartRescan(string id)
        {
            if (string.IsNullOrEmpty(_fileRepository.GetTaskIdForScanner(id)))
                return Forbid("Rescan is active right now.");
            Scanner scanner = _fileRepository.GetScanner(id);
            ScanSettings settings = _fileRepository.GetScanSettings(id);
            using HttpClient httpClient = new ();
            HttpResponseMessage response = httpClient.Send(new HttpRequestMessage
            {
                RequestUri = new Uri($"{scanner.Host}/Scan/Rescan"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(settings))
            });
            if (response.IsSuccessStatusCode)
            {
                _fileRepository.AddTaskIdForScanner(id, response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                return Ok();
            }
            else
                return BadRequest(response.Content);
        }

        [HttpGet]
        [Route("RescanTaskId/{id}")]
        public IActionResult GetRescanTaskId(string id)
        {
            return Ok(new { id = _fileRepository.GetTaskIdForScanner(id) ?? "" });
        }

        [HttpPost]
        [Route("End/{id}")]
        public IActionResult EndScan(string id)
        {
            _fileRepository.DeleteTaskIdForScanner(id);
            return Ok();
        }

    }
}
