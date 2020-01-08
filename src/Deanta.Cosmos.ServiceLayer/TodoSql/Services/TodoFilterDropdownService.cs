using System;
using System.Collections.Generic;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql.QueryObjects;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Services
{
    public class TodoFilterDropdownService : ITodoFilterDropdownService
    {
        private readonly DeantaSqlDbContext _db;

        public TodoFilterDropdownService(DeantaSqlDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public IEnumerable<DropdownTuple> GetFilterDropDownValues(TodosFilterBy filterBy)
        {
            switch (filterBy)
            {
                case TodosFilterBy.NoFilter:
                    //return an empty list
                    return new List<DropdownTuple>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
            }
        }
    }
}