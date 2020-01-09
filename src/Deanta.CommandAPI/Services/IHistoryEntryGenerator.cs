using System.Collections.Generic;
using Deanta.Models.Contracts;
using Deanta.Models.Models;
using HistoryEntry = Deanta.Models.Models.HistoryEntry;
using User = Deanta.Models.Models.User;

namespace Deanta.CommandAPI.Services
{
    public interface IHistoryEntryGenerator
    {
        List<HistoryEntry> GetHistoryEntriesForCreation(
            string name, User user, AuditableEntity model);

        List<HistoryEntry> GetHistoryEntriesForModification(
            string name, User user, AuditableEntity oldEntity
            , AuditableEntity newEntity);

        List<HistoryEntry> GetHistoryEntriesForDeletion(
            string name, User user, AuditableEntity model);
    }


}
