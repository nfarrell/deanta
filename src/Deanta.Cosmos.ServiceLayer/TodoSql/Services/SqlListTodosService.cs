using System.Linq;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.QueryObjects;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;
using Deanta.Cosmos.ServiceLayer.TodoSql.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Services
{
    public class SqlListTodosService : ISqlListTodosService
    {
        private readonly DeantaSqlDbContext _context;

        public SqlListTodosService(DeantaSqlDbContext context)
        {
            _context = context;
        }

        public IQueryable<TodoListDto> SortFilterPage(SqlSortFilterPageOptions options)
        {
            var todosQuery = _context.Todos            
                .AsNoTracking()                        
                .MapTodoToDto()                        
                .OrderTodosBy(options.OrderByOptions)  
                .FilterTodosBy(options.FilterBy,       
                               options.FilterValue);   

            options.SetupRestOfDto(todosQuery);        

            return todosQuery.Page(options.PageNum-1,  
                                   options.PageSize);  
        }
    }

}