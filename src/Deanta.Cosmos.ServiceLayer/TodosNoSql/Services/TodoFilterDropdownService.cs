
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodosNoSql.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql.Services
{
    public class TodoFilterDropdownService : ITodoNoSqlFilterDropdownService
    {
        private readonly NoDeantaSqlDbContext _db;

        public TodoFilterDropdownService(NoDeantaSqlDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DropdownTuple>> GetFilterDropDownValuesAsync(TodosFilterBy filterBy)
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