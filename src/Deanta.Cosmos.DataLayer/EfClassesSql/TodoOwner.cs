using System;

namespace Deanta.Cosmos.DataLayer.EfClassesSql
{
    public class TodoOwner : ITodoId
    {
        private TodoOwner() { }

        internal TodoOwner(Todo todo, Owner owner, byte order)
        {
            Todo = todo;
            Owner = owner;
            Order = order;
        }

        public Guid OwnerId { get; private set; }
        public byte Order { get; }

        //-----------------------------
        //Relationships

        public Todo Todo { get; }
        public Owner Owner { get; }

        public Guid TodoId { get; private set; }
    }
}