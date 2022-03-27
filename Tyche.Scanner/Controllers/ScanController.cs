using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Tyche.Scanner.Models;
using Tyche.Scanner.Workers;
using Tyche.Shared.Models;

namespace Tyche.Scanner.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ScanController : ControllerBase
    {
        readonly TaskWorker _taskWorker;
        readonly SettingsProvider _settingsProvider;
        readonly ILogger _logger;

        public ScanController(TaskWorker taskWorker, SettingsProvider settingsProvider, ILogger<ScanController> logger)
        {
            _taskWorker = taskWorker;
            _settingsProvider = settingsProvider;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Status()
        {
            return new JsonResult(new 
            {
                status = "Active",
                activeTasks = _taskWorker.QueuedTasks
            });
        }

        [HttpGet]
        public IActionResult Settings()
        {
            _logger.LogInformation("Get settings");
            return new JsonResult(_settingsProvider.GetSettings());
        }

        [HttpPost]
        public IActionResult Rescan([FromBody] ScanSettings scanSettings)
        {
            WebWorker webWorker = new(Request.Host.Value, Request.IsHttps);
            ScanWorker scanWorker = new(_settingsProvider, webWorker, scanSettings);
            string scanId = _taskWorker.AddTask(webWorker, scanWorker.ScanDirectory, scanSettings.ScannerId);
            _logger.LogInformation($"Scan started with guid: {scanId}");
            return new JsonResult(scanId);
        }

        [HttpGet("NewFiles/{includeSubfolders}")]
        public IActionResult NewFiles(bool includeSubfolders = true)
        {
            WebWorker webWorker = new(Request.Host.Value, Request.IsHttps);
            ScanWorker scanWorker = new(_settingsProvider, webWorker, new ScanSettings());
            var files = scanWorker.GetFiles(includeSubfolders).Select(f => new { fileName = f.FullName, fileSize = f.Length }).ToArray();
            return new JsonResult(new 
            {
                count = files.Length,
                files
            });
        }
    }
}
