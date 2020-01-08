using System;
using System.Linq;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.QueryObjects
{
    public static class TodoListDtoSort
    {
        public static IQueryable<TodoListDto> OrderTodosBy
            (this IQueryable<TodoListDto> todos, OrderByOptions orderByOptions)
        {
            return orderByOptions switch
            {
                OrderByOptions.ByCreatedDate => todos.OrderByDescending(x => x.CreatedAt),
                _ => throw new ArgumentOutOfRangeException(nameof(orderByOptions), orderByOptions, null)
            };
        }
    }

}