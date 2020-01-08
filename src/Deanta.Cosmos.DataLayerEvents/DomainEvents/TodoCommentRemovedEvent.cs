
using System;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using GenericEventRunner.ForEntities;

namespace Deanta.Cosmos.DataLayerEvents.DomainEvents
{
    public class TodoCommentRemovedEvent : IDomainEvent
    {
        public TodoCommentRemovedEvent(CommentWithEvents commentRemoved, TodoWithEvents todo, Action<int> updateCommentCachedValues)
        {
            CommentRemoved = commentRemoved;
            Todo = todo;
            UpdateCommentCachedValues = updateCommentCachedValues;
        }

        public CommentWithEvents CommentRemoved { get; }

        public TodoWithEvents Todo { get; }

        public Action<int> UpdateCommentCachedValues { get; }
    }
}