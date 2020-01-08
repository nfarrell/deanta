
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using System;
using System.Collections.Generic;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Services
{
    public class TodoEventsFilterDropdownService : ITodoEventsFilterDropdownService
    {
        private readonly SqlEventsDbContext _db;

        public TodoEventsFilterDropdownService(SqlEventsDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public IEnumerable<DropdownTuple> GetFilterDropDownValues(TodosFilterBy filterBy)
        {
            return filterBy switch
            {
                TodosFilterBy.NoFilter =>
                //return an empty list
                new List<DropdownTuple>(),
                TodosFilterBy.ByCompleted => FormCompletedDropDown(),
                _ => throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null)
            };
        }

        private static IEnumerable<DropdownTuple> FormCompletedDropDown()
        {
            return new[]
            {
                new DropdownTuple {Value = "All", Text = "All"},
                new DropdownTuple {Value = "NotCompleted", Text = "Not Completed"},
                new DropdownTuple {Value = "Completed", Text = "Completed"}
            };
        }
    }
}