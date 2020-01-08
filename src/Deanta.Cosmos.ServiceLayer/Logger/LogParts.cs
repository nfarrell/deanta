
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Deanta.Cosmos.ServiceLayer.Logger
{
    public class LogParts
    {
        private const string EfCoreEventIdStartWith = "Microsoft.EntityFrameworkCore";

        public LogParts(LogLevel logLevel, EventId eventId, string eventString)
        {
            LogLevel = logLevel;
            EventId = eventId;
            EventString = eventString;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; }

        public EventId EventId { get; }

        public string EventString { get; }

        public bool IsDb
        {
            get
            {
                var name = EventId.Name;
                return name != null && (name.StartsWith(EfCoreEventIdStartWith));
            }
        }

        public override string ToString()
        {
            return $"{LogLevel}: {EventString}";
        }
    }
}