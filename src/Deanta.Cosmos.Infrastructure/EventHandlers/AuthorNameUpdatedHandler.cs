using System.Linq;
using Deanta.Cosmos.DataLayerEvents.DomainEvents;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using GenericEventRunner.ForEntities;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace Deanta.Cosmos.Infrastructure.EventHandlers
{
    public class OwnerNameUpdatedHandler : IBeforeSaveEventHandler<OwnerNameUpdatedEvent>
    {
        private readonly SqlEventsDbContext _context;

        public OwnerNameUpdatedHandler(SqlEventsDbContext context)
        {
            _context = context;
        }

        public IStatusGeneric Handle(EntityEvents callingEntity, OwnerNameUpdatedEvent domainEvent)
        {
            foreach (var todoWithEvents in _context.TodoOwners
                .Where(x => x.OwnerId == domainEvent.ChangedOwner.OwnerId)
                .Select(x => x.Todo))
            {
                //For each todo that has this author has its OwnersOrdered string recomputed.
                var allOwnersInOrder = _context.Set<TodoWithEvents>()
                    .Where(x => x.TodoId == todoWithEvents.TodoId)
                    .Select(x => x.OwnersLink.OrderBy(y => y.Order).Select(y => y.Owner).ToList())
                    .Single();

                //The database hasn't been updated yet, so we have to manually insert the new name into the correct point in the ownersOrdered
                var newOwnersOrdered = string.Join(", ", allOwnersInOrder.Select(x =>
                    x.OwnerId == domainEvent.ChangedOwner.OwnerId
                        ? domainEvent.ChangedOwner.Name
                        : x.Name));

                todoWithEvents.OwnersOrdered = newOwnersOrdered;
            }

            return null;
        }

    }
}