
using System;
using System.Linq;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.QueryObjects
{
    public static class TodoEventsListDtoSort
    {
        public static IQueryable<TodoListDto> OrderTodosEventsBy
            (this IQueryable<TodoListDto> todos, OrderByOptions orderByOptions)
        {
            return orderByOptions switch
            {
                OrderByOptions.ByCreatedDate => todos.OrderByDescending(x => x.CreatedAt),
            };
        }
    }

}