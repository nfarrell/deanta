using Deanta.Cosmos.DataLayerEvents.DomainEvents;
using GenericEventRunner.ForEntities;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace Deanta.Cosmos.Infrastructure.EventHandlers
{
    public class CommentRemovedHandler : IBeforeSaveEventHandler<TodoCommentRemovedEvent>
    {
        public IStatusGeneric Handle(EntityEvents callingEntity, TodoCommentRemovedEvent domainEvent)
        {
            //This method is just to show how we handle in realtime a user updating number of comments

            //Here is the fast (delta) version of the update. Doesn't need access to the database
            var numComments = domainEvent.Todo.CommentsCount - 1;
            domainEvent.UpdateCommentCachedValues(numComments);

            return null;
        }
    }
}