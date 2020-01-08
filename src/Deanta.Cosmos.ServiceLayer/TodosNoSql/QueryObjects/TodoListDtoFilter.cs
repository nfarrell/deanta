using System;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;
using Deanta.Cosmos.ServiceLayer.TodosCommon;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql.QueryObjects
{
    public static class TodoListDtoFilter
    {
        public const string AllTodosNotPublishedString = "<<Draft>>"; //retains the state

        public static IQueryable<TodoListNoSql> FilterTodosBy(
            this IQueryable<TodoListNoSql> todos,
            TodosFilterBy filterBy, string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
                return todos;

            return filterBy switch
            {
                TodosFilterBy.NoFilter => todos,
                TodosFilterBy.ByCompleted =>
                //var filterVote = int.Parse(filterValue);     
                todos.Where(x => x.IsCompleted == true),
                _ => throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null)
            };
        }
    }
}