using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayer.EfClassesSql
{
    public class Todo : AuditableEntity, ITodoId
    {
        private HashSet<TodoOwner> _ownersLink; //It's a link because of Cosmos architecture :(

        private HashSet<Comment> _comments;

        private Todo()
        {
        }

        [Required(AllowEmptyStrings = false)] public string Title { get; private set; }

        //todo: implement Later
        public string Description { get; private set; }

        public bool SoftDeleted { get; set; }

        public IEnumerable<Comment> Comments => _comments?.ToList();
        public IEnumerable<TodoOwner> OwnersLink => _ownersLink?.ToList();

        public Guid TodoId { get; private set; }

        public static IStatusGeneric<Todo> CreateTodo(string title, ICollection<Owner> owner)
        {
            var status = new StatusGenericHandler<Todo>();
            if (string.IsNullOrWhiteSpace(title))
                status.AddError("The todo title cannot be empty.");

            var todo = new Todo
            {
                TodoId = Guid.NewGuid(),
                Title = title,
                //Description = description,
                _comments = new HashSet<Comment>()
            };

            if (owner == null)
                throw new ArgumentNullException(nameof(owner), "A todo item must have at least one owner");

            return status.SetResult(todo);
        }

        #region Comments - Not Implemented

        public void AddComment(string comment, string commentOwner, DbContext context = null)
        {
            throw new NotImplementedException("N.F. Left this bit on to prove extensability of code.");

            if (_comments != null)
            {
                _comments.Add(new Comment(commentOwner));
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context),
                    "You must provide a context if the Comments collection isn't valid.");
            }
            else if (context.Entry(this).IsKeySet)
            {
                context.Add(new Comment(commentOwner, TodoId));
            }
            else
            {
                throw new InvalidOperationException("Could not add a new comment.");
            }
        }

        public void RemoveComment(int commentId, DbContext context = null)
        {
            throw new NotImplementedException("N.F. Left this bit on to prove extensability of code.");

            if (_comments != null)
            {
                //This is there to handle the add/remove of comments when first created (or someone uses an .Include(p => p.Comments)
                var localComment = _comments.SingleOrDefault(x => x.CommentId == commentId);
                if (localComment == null)
                    throw new InvalidOperationException(
                        "The comment with that key was not found in the todo's Comments.");
                _comments.Remove(localComment);
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context),
                    "You must provide a context if the Comments collection isn't valid.");
            }
            else
            {
                var comment = context.Find<Comment>(commentId);
                if (comment == null || comment.TodoId != TodoId)
                {
                    // Ensures comment has a valid primary key
                    throw new InvalidOperationException(
                        "The comment either wasn't found or was not linked to this Todo.");
                }
                context.Remove(comment);
            }
        }

        #endregion
    }
}