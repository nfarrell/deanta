using System;

namespace Deanta.Models.Contracts
{
    public class HistoryEntry
    {
        public Guid Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public User User { get; set; }

        public string Tags { get; set; }

        public Guid DeantaId { get; set; }

        public string Description { get; set; }
    }
}
