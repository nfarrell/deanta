using System;
using System.Linq;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.QueryObjects
{
    public static class TodoListDtoFilter
    {
        public const string AllTodosNotPublishedString = "Draft";

        public static IQueryable<TodoListDto> FilterTodosBy(
            this IQueryable<TodoListDto> todos,
            TodosFilterBy filterBy, string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
                return todos;

            return filterBy switch
            {
                TodosFilterBy.NoFilter => todos,
                _ => throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null)
            };
        }

        /***************************************************************
        #A The method is given both the type of filter and the user selected filter value
        #B If the filter value isn't set then it returns the IQueryable with no change
        #C Same for no filter selected - it returns the IQueryable with no change
        #D The filter by votes is a value and above, e.g. 3 and above. Note: not comments returns null, and the test is always false
        #E If the "coming soon" was picked then we only return todos not yet published
        #F If we have a specific year we filter on that. Note that we also remove future todos (in case the user chose this year's date)
         * ************************************************************/
    }
}