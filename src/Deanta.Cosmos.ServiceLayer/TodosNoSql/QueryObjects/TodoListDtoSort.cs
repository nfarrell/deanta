
using System;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;
using Deanta.Cosmos.ServiceLayer.TodosCommon;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql.QueryObjects
{

    public static class TodoListDtoSort
    {
        public static IQueryable<TodoListNoSql> OrderTodosBy (this IQueryable<TodoListNoSql> todos, 
             OrderByOptions orderByOptions)
        {
            return orderByOptions switch
            {
                OrderByOptions.ByCreatedDate => todos.OrderByDescending(x => x.CreatedAt),
                _ => throw new ArgumentOutOfRangeException(nameof(orderByOptions), orderByOptions, null)
            };
        }
    }

}