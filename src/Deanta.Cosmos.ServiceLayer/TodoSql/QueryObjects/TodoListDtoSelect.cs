using System.Linq;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.QueryObjects
{
    public static class TodoListDtoSelect
    {
        public static IQueryable<TodoListDto> MapTodoToDto(this IQueryable<Todo> todos)
        {
            return todos.Select(p => new TodoListDto
            {
                TodoId = p.TodoId,
                Title = p.Title,
                OwnersOrdered = string.Join(", ", p.OwnersLink.Select(x => x.Owner.Name)),
            });
        }
    }
}