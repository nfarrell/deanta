using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.DomainEvents;
using GenericEventRunner.ForEntities;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayerEvents.EfClasses
{
    public class TodoWithEvents : EntityEvents
    {
        private HashSet<TodoOwnerWithEvents> _ownerLink;

        //Use uninitialized backing fields - this means we can detect if the collection was loaded
        private HashSet<CommentWithEvents> _comments;

        private TodoWithEvents() { } //Needed by EF Core

        [Required(AllowEmptyStrings = false)] //we want all tasks to have an associated description.
        public string Title { get; private set; }

        public bool SoftDeleted { get; private set; }

        public IEnumerable<CommentWithEvents> Comments => _comments?.ToList();
        public IEnumerable<TodoOwnerWithEvents> OwnersLink => _ownerLink?.ToList();

        public Guid TodoId { get; private set; }

        [ConcurrencyCheck]
        public string OwnersOrdered { get; set; }

        [ConcurrencyCheck]
        public int CommentsCount { get; set; }

        //This is an action provided in the comment add/remove event so that the comment handler can update these properties
        private void UpdateCommentCachedValues(int commentsCount)
        {
            CommentsCount = commentsCount;
        }
        //----------------------------------------------

        public static IStatusGeneric<TodoWithEvents> CreateTodo(string title, ICollection<OwnerWithEvents> owners)
        {
            var status = new StatusGenericHandler<TodoWithEvents>();
            if (string.IsNullOrWhiteSpace(title))
                status.AddError("The todoWithEvents title cannot be empty.");

            var todo = new TodoWithEvents
            {
                TodoId = Guid.NewGuid(),
                Title = title,
                //We need to initialise the OwnersOrdered string when the entry is created
                OwnersOrdered = string.Join(", ", owners.Select(x => x.Name)),
                _comments = new HashSet<CommentWithEvents>()       //We add an empty list on create. I allows comments to be added when building test data
            };
            if (owners == null)
                throw new ArgumentNullException(nameof(owners));

            byte order = 0;
            todo._ownerLink = new HashSet<TodoOwnerWithEvents>(owners.Select(a => new TodoOwnerWithEvents(todo, a, order++)));
            if (!todo._ownerLink.Any())
                status.AddError("You must have at least one Owner for a todoWithEvents.");

            return status.SetResult(todo);
        }

        public void AddComment(string comment, string commentOwner,
            DbContext context = null)
        {
            if (_comments != null)
            {
                _comments.Add(new CommentWithEvents(comment, commentOwner));
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context),
                    "You must provide a context if the Comments collection isn't valid.");
            }
            else if (context.Entry(this).IsKeySet)
            {
                context.Add(new CommentWithEvents(comment, commentOwner, TodoId));
            }
            else
            {
                throw new InvalidOperationException("Could not add a new comment.");
            }

            AddEvent(new TodoCommentAddedEvent(this, UpdateCommentCachedValues));
        }

        public void RemoveComment(int commentId, DbContext context = null)
        {
            CommentWithEvents comment;
            if (_comments != null)
            {
                //This is there to handle the add/remove of comments when first created (or someone uses an .Include(p => p.Comments)
                comment = _comments.SingleOrDefault(x => x.CommentId == commentId);
                if (comment == null)
                    throw new InvalidOperationException("The comment with that key was not found in the todo's Comments.");
                _comments.Remove(comment);
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context),
                    "You must provide a context if the Comments collection isn't valid.");
            }
            else
            {
                comment = context.Find<CommentWithEvents>(commentId);
                if (comment == null || comment.TodoId != TodoId)
                {
                    // This ensures that the comment is a) linked to the todo you defined, and b) the comment has a valid primary key
                    throw new InvalidOperationException("The comment either wasn't found or was not linked to this Todo.");
                }

                context.Remove(comment);
            }

            AddEvent(new TodoCommentRemovedEvent(comment, this, UpdateCommentCachedValues));
        }
    }
}