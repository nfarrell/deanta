using System;
using System.Linq;
using System.Text;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.DataLayerEvents.QueryExtensions;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Services
{
    public class HardResetCacheService : IHardResetCacheService
    {
        [Flags]
        private enum Errors
        {
            None, 
            OwnersWrong = 1,
        }

        private readonly SqlEventsDbContext _context;

        public HardResetCacheService(SqlEventsDbContext context)
        {
            _context = context;
        }

        public IStatusGeneric<string> CheckUpdateTodoCacheProperties()
        {
            var status = new StatusGenericHandler<string>();
            var errorStrings = new StringBuilder();
            var numTodosChecked = 0;
            var numErrors = 0;
            foreach (var todo in _context.Todos
                .Include(x => x.Comments)
                .Include(x => x.OwnersLink).ThenInclude(x => x.Owner))
            {
                var error = Errors.None;
                numTodosChecked++;

                var ownersOrdered = todo.FormOwnerOrderedString();
                var commentsCount = todo.Comments.Count();

                if (ownersOrdered != todo.OwnersOrdered)
                {
                    todo.OwnersOrdered = ownersOrdered;
                    error = Errors.OwnersWrong;
                }
                if (error != Errors.None)
                {
                    errorStrings.AppendLine($"Todo: {todo.Title} had the following errors: {error.ToString()}");
                    numErrors++;
                }
            }

            if (numErrors > 0)
                _context.SaveChanges();

            status.SetResult(errorStrings.ToString());
            status.Message = numErrors == 0
                ? $"Processed {numTodosChecked} and no cache errors found"
                : $"Processed {numTodosChecked} todos and {numErrors} errors found. See returned string for details";

            return status;
        }

    }
}