
using System;
using System.Linq;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.QueryObjects
{
    public static class TodoEventsListDtoFilter
    {
        public static IQueryable<TodoListDto> FilterTodosEventsBy(
            this IQueryable<TodoListDto> todos,
            TodosFilterBy filterBy, string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
                return todos;

            switch (filterBy)
            {
                case TodosFilterBy.NoFilter:
                    return todos;
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }
    }
}