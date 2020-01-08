using System.Collections.Generic;
using Deanta.Cosmos.ServiceLayer.TodosCommon;

namespace Deanta.Cosmos.ServiceLayer.TodoSql
{
    public interface ITodoFilterDropdownService
    {
        IEnumerable<DropdownTuple> GetFilterDropDownValues(TodosFilterBy filterBy);
    }
}