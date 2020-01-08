using System.Linq;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;

namespace Deanta.Cosmos.ServiceLayer.TodoSql
{
    public interface ISqlListTodosService
    {
        IQueryable<TodoListDto> SortFilterPage(SqlSortFilterPageOptions options);
    }
}