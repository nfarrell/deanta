using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Deanta.Cosmos.Test")]

namespace Deanta.Cosmos.DataLayerEvents.EfClasses
{
    public class CommentWithEvents
    {
        public const int NameLength = 100;

        private CommentWithEvents() { }

        internal CommentWithEvents(string comment, string commentOwner, Guid todoId = default)
        {
            Comment = comment;
            CommentOwner = commentOwner;
            if (todoId != default)
                TodoId = todoId;
        }

        [Key]
        public int CommentId { get; private set; }

        [MaxLength(NameLength)]
        public string CommentOwner { get; }

        public string Comment { get; }

        public Guid TodoId { get; }
    }

}