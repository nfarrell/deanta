
using System.Collections.Generic;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.QueryObjects;
using Deanta.Cosmos.ServiceLayer.TodosNoSql.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql.Services
{
    public class ListNoSqlTodosService : IListNoSqlTodosService
    {
        private readonly NoDeantaSqlDbContext _context;

        public ListNoSqlTodosService(NoDeantaSqlDbContext context)
        {
            _context = context;
        }

        public async Task<IList<TodoListNoSql>> SortFilterPageAsync(NoSqlSortFilterPageOptions options)
        {
            var todosFound = await _context.Todos
                .AsNoTracking()                                             
                .OrderTodosBy(options.OrderByOptions)  
                .FilterTodosBy(options.FilterBy,       
                               options.FilterValue)
                .Page(options.PageNum - 1,options.PageSize)
                .ToListAsync();   

            options.SetupRestOfDto(todosFound.Count);

            return todosFound;
        }
    }

}