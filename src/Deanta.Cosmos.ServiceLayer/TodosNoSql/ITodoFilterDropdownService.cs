
using System.Collections.Generic;
using System.Threading.Tasks;
using Deanta.Cosmos.ServiceLayer.TodosCommon;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql
{
    public interface ITodoNoSqlFilterDropdownService
    {
        Task<IEnumerable<DropdownTuple>> GetFilterDropDownValuesAsync(TodosFilterBy filterBy);
    }
}