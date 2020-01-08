
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using GenericEventRunner.ForEntities;

namespace Deanta.Cosmos.DataLayerEvents.DomainEvents
{
    public class OwnerNameUpdatedEvent : IDomainEvent
    {
        public OwnerNameUpdatedEvent(OwnerWithEvents changedOwner)
        {
            ChangedOwner = changedOwner;
        }

        public OwnerWithEvents ChangedOwner { get; }
    }
}