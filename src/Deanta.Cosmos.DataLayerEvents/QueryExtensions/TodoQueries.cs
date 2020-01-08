using System;
using System.Collections.Generic;
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.DataLayerEvents.QueryExtensions
{
    public static class TodoQueries
    {
        public static string FormOwnerOrderedString(this DbContext context, Guid todoId)
        {
            return context.Set<TodoWithEvents>()
                .Where(x => x.TodoId == todoId)
                .Select(x => x.OwnersLink.OrderBy(y => y.Order).Select(y => y.Owner.Name).ToList())
                .Single().FormOwnerOrderedString();
        }

        public static string FormOwnerOrderedString(this TodoWithEvents todo)
        {
            if (todo.OwnersLink == null || todo.OwnersLink.Any(x => x.Owner == null))
                throw new InvalidOperationException("The todo must have the OwnerLink collection filled, and the OwnerLink Owner filled too.");

            return todo.OwnersLink.OrderBy(x => x.Order).Select(x => x.Owner.Name).FormOwnerOrderedString();
        }

        public static string FormOwnerOrderedString(this IEnumerable<string> orderedNames)
        {
            return string.Join(", ", orderedNames);
        }

        public static int CalcCommentCountCacheValuesFromDb(this DbContext context, Guid todoId)
        {
            var commentData = context.Set<CommentWithEvents>()
                .Where(x => x.TodoId == todoId).Select(x => 0) //todo Implement Comment Count
                .ToList();

            return commentData.Count;
        }

        /// <summary>
        /// The logic of this method is completely pointless, but just to demonstrate use of Tuples<>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="todoId"></param>
        /// <returns></returns>
        public static (int CommentCount, double NumberOfParticipants) CalcTaskParticipantsCacheValuesFromDb(
            this DbContext context, Guid todoId)
        {
            var commentData = context.Set<CommentWithEvents>()
                .Where(x => x.TodoId == todoId).Select(x => 1)
                //todo Implement Get Unique number of participants
                .ToList();

            return (CommentCount: commentData.Count, NumberOfParticipants: 1);
        }
    }
}