using System;
using System.Collections.Generic;
using Deanta.Models.Enums;

namespace Deanta.Models.Models
{
    public class HistoryEntry
    {
        public HistoryEntry()
        {
        }

        public HistoryEntry(Guid id, DateTimeOffset timeStamp, User user, Guid DeantaId,
            string name, string description, string tags, List<Attribute> attributes, TodoItemEnums.HistoryEntryType historyEntryType)
        {
            Id = id;
            Timestamp = timeStamp;
            User = user;
            DeantaId = DeantaId;
            Name = name;
            Description = description;
            Tags = tags;
            Attributes = attributes;
            HistoryEntryType = historyEntryType;
        }

        public Guid Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public User User { get; set; }

        public Guid DeantaId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        public List<Attribute> Attributes { get; set; }

        public TodoItemEnums.HistoryEntryType HistoryEntryType { get; set; }
    }
}
