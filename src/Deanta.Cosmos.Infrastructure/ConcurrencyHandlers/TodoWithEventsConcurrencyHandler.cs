using System;
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace Deanta.Cosmos.Infrastructure.ConcurrencyHandlers
{
    public static class TodoWithEventsConcurrencyHandler
    {
        public static IStatusGeneric HandleCacheValuesConcurrency(this Exception ex, DbContext context)
        {
            var dbUpdateEx = ex as DbUpdateConcurrencyException;
            if (dbUpdateEx == null)
                return null; //can't handle this error
            
            var status = new StatusGenericHandler();

            //There could be multiple todos if there was a bulk upload. Unusual, but best to handle it.
            foreach (var entry in dbUpdateEx.Entries)
            {
                if (!(entry.Entity is TodoWithEvents todoBeingWrittenOut))
                    return null; //This handler only handles TodoWithEvents

                //we read in the todo that caused the concurrency issue
                //This MUST be read as NoTracking otherwise it will interfere with the same entity we are trying to write
                var todoThatCausedConcurrency = context.Set<TodoWithEvents>().AsNoTracking()
                    .SingleOrDefault(p => p.TodoId == todoBeingWrittenOut.TodoId);

                if (todoThatCausedConcurrency == null)
                {
                    //The todo was deleted so we need to stop the todo being written out
                    entry.State = EntityState.Detached;
                    continue;
                }

                var handler = new FixConcurrencyMethods(entry, context);

                ////todo: implement how we re-map back to the database object - ideally in nameof(FixConcurrencyMethods)
                //handler.CheckFixCommentCacheValues(todoThatCausedConcurrency, todoBeingWrittenOut);
                //handler.CheckFixOwnerOrdered(todoThatCausedConcurrency, todoBeingWrittenOut);
            }

            return status; //We return a status with no errors, which tells the caller to retry the SaveChanges
        }
    }
}