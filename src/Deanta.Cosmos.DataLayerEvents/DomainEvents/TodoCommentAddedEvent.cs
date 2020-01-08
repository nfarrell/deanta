
using System;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using GenericEventRunner.ForEntities;

namespace Deanta.Cosmos.DataLayerEvents.DomainEvents
{
    /// <summary>
    /// Using this an updated event to notify all the owners that there's been a change
    /// </summary>
    public class TodoCommentAddedEvent : IDomainEvent
    {
        public TodoCommentAddedEvent(TodoWithEvents todo, Action<int> updateCommentCachedValues)
        {
            Todo = todo;
        }

        public TodoWithEvents Todo { get; }
    }
}