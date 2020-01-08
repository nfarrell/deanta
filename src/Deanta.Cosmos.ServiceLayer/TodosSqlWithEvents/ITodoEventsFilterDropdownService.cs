
using System.Collections.Generic;
using Deanta.Cosmos.ServiceLayer.TodosCommon;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents
{
    public interface ITodoEventsFilterDropdownService
    {
        IEnumerable<DropdownTuple> GetFilterDropDownValues(TodosFilterBy filterBy);
    }
}