
using System.Collections.Generic;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql
{
    public class NoSqlTodoListCombinedDto
    {
        public NoSqlTodoListCombinedDto(NoSqlSortFilterPageOptions sortFilterPageData, IList<TodoListNoSql> todosList)
        {
            SortFilterPageData = sortFilterPageData;
            TodosList = todosList;
        }

        public NoSqlSortFilterPageOptions SortFilterPageData { get; }

        public IList<TodoListNoSql> TodosList { get; }
    }
}