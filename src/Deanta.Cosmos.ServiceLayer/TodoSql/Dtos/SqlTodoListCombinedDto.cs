using System.Collections.Generic;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    public class SqlTodoListCombinedDto
    {
        public SqlTodoListCombinedDto(SqlSortFilterPageOptions sortFilterPageData, IEnumerable<TodoListDto> todosList)
        {
            SortFilterPageData = sortFilterPageData;
            TodosList = todosList;
        }

        public SqlSortFilterPageOptions SortFilterPageData { get; }

        public IEnumerable<TodoListDto> TodosList { get; }
    }
}