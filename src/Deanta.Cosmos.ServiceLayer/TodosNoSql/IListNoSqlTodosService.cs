
using System.Collections.Generic;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql
{
    public interface IListNoSqlTodosService
    {
        Task<IList<TodoListNoSql>> SortFilterPageAsync(NoSqlSortFilterPageOptions options);
    }
}