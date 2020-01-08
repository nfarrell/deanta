using System;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Cosmos.DataLayer.EfClassesSql
{
    public class Comment : ITodoId
    {
        public const int NameLength = 100;

        private Comment() { }

        internal Comment(string commentOwner, Guid todoId = default)
        {
            CommentOwner = commentOwner;

            if (todoId != default)
                TodoId = todoId;
        }

        public int CommentId { get; private set; }

        [MaxLength(NameLength)]
        public string CommentOwner { get; }

        public string CommentDetail { get; set; }

        public Guid TodoId { get; }
    }

}