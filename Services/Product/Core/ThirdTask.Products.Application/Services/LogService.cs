using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThirdTask.Products.Application.Interfaces;

namespace ThirdTask.Products.Application.Services
{
    public class LogService : ILogService
    {
        private readonly HttpClient _httpClient;

        public LogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task LogAsync(string serviceName, string level, string message)
        {
            var logEntry = new
            {
                ServiceName = serviceName,
                Level = level,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(logEntry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("http://localhost:6000/services/logs", content);
        }
        public async Task LogAsyncExecute(string serviceName, Exception ex, bool isCritical = false)
        {
            var level = isCritical ? "CRITICAL" : "ERROR";
            var message = ex.Message + (ex.StackTrace != null ? $" | StackTrace: {ex.StackTrace}" : "");

            await LogAsync(serviceName, level, message);
        }
    }
}
