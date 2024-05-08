namespace DataCollector.Models.Shared
{
    public class RequestResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
