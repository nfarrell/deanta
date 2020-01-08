
using System.Linq;
using Deanta.Cosmos.ServiceLayer.TodoSql;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents
{
    public interface ISqlEventsListTodosService
    {
        IQueryable<TodoListDto> SortFilterPage(SqlSortFilterPageOptions options);
    }
}