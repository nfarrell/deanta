
using System;

namespace Deanta.Cosmos.DataLayerEvents.EfClasses
{
    public class TodoOwnerWithEvents
    {
        private TodoOwnerWithEvents() { }

        internal TodoOwnerWithEvents(TodoWithEvents todoWithEvents, OwnerWithEvents ownerWithEvents, byte order)
        {
            Todo = todoWithEvents;
            Owner = ownerWithEvents;
            Order = order;
        }

        public Guid TodoId { get; private set; }
        public Guid OwnerId { get; private set; }
        public byte Order { get; }

        //-----------------------------
        //Relationships

        public TodoWithEvents Todo { get; }
        public OwnerWithEvents Owner { get; }
    }
}