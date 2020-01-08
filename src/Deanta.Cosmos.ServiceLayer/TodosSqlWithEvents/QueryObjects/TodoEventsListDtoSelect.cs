
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.QueryObjects
{
    public static class TodoEventsListDtoSelect
    {
        public static IQueryable<TodoListDto> MapTodoEventsToDto(this IQueryable<TodoWithEvents> todos)     
        {
            return todos.Select(p => new TodoListDto
            {
                TodoId = p.TodoId,                        
                Title = p.Title,                                                  
                OwnersOrdered = p.OwnersOrdered,
                
            });
        }
    }
}