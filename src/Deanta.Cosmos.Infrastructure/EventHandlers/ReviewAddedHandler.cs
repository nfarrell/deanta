
using System;
using Deanta.Cosmos.DataLayerEvents.DomainEvents;
using GenericEventRunner.ForEntities;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace Deanta.Cosmos.Infrastructure.EventHandlers
{
    public class CommentAddedHandler : IBeforeSaveEventHandler<TodoCommentAddedEvent>
    {
        public IStatusGeneric Handle(EntityEvents callingEntity, TodoCommentAddedEvent domainEvent)
        {
            //todo: SendGrid - email all users!

            return null;
        }
    }
}