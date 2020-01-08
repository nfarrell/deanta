namespace Deanta.Cosmos.ServiceLayer.Logger
{
    public class TraceIdentBaseDto
    {
        public TraceIdentBaseDto(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
            NumLogs = HttpRequestLog.GetHttpRequestLog(traceIdentifier).RequestLogs.Count;
        }

        public string TraceIdentifier { get; }

        public int NumLogs { get; }
    }
}