using System;
using System.Collections.Generic;
using Deanta.Models.Enums;
using Deanta.Models.Models;
using Deanta.Models.Toolbox;

namespace Deanta.CommandAPI.Services
{
    public class HistoryEntryGenerator : IHistoryEntryGenerator
    {
        public List<HistoryEntry> GetHistoryEntriesForCreation(
            string name, User user, AuditableEntity entity)
        {
            var result = new List<HistoryEntry>();
            var deantaInformation = VerboseLogger.Log(entity);

            return result;
        }

        public List<HistoryEntry> GetHistoryEntriesForModification(
            string name, User user, AuditableEntity oldEntity, AuditableEntity newEntity)
        {
            var result = new List<HistoryEntry>();
            var deantaInformation = VerboseLogger.Log(oldEntity, newEntity);
            result.Add(new HistoryEntry()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = deantaInformation,
                HistoryEntryType = TodoItemEnums.HistoryEntryType.Create,
                Timestamp = DateTimeOffset.Now,
                User = user
            });
            return result;
        }

        public List<HistoryEntry> GetHistoryEntriesForDeletion(
            string name, User user, AuditableEntity entity)
        {
            var result = new List<HistoryEntry>();
            result.Add(new HistoryEntry()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = string.Empty, //todo: implement logic - perhaps move ID to parent entity? Or re-implement StatusEntity
                HistoryEntryType = TodoItemEnums.HistoryEntryType.Update,
                Timestamp = DateTimeOffset.Now,
                User = user
            });
            return result;
        }
    }
}
