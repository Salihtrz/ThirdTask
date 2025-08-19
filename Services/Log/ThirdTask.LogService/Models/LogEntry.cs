namespace ThirdTask.LogService.Models
{
    public class LogEntry
    {
        public string ServiceName { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
