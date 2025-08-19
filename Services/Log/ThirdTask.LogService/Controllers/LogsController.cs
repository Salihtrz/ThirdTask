using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using ThirdTask.LogService.Models;

namespace ThirdTask.LogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateLog([FromBody] LogEntry logEntry)
        {
            switch (logEntry.Level.ToUpper())
            {
                case "INFO":
                    Log.Information("[{Service}] {Message}", logEntry.ServiceName, logEntry.Message);
                    break;
                case "WARNING":
                    Log.Warning("[{Service}] {Message}", logEntry.ServiceName, logEntry.Message);
                    break;
                case "ERROR":
                    Log.Error("[{Service}] {Message}", logEntry.ServiceName, logEntry.Message);
                    break;
                case "CRITICAL":
                    Log.Fatal("[{Service}] {Message}", logEntry.ServiceName, logEntry.Message);
                    break;
                default:
                    Log.Information("[{Service}] {Message}", logEntry.ServiceName, logEntry.Message);
                    break;
            }

            return Ok();
        }
    }
}
