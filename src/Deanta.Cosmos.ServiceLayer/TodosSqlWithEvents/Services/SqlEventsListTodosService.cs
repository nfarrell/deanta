
using System.Linq;
using Deanta.Cosmos.DataLayer.QueryObjects;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.ServiceLayer.TodoSql;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;
using Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Services
{
    public class SqlEventsListTodosService : ISqlEventsListTodosService
    {
        private readonly SqlEventsDbContext _context;

        public SqlEventsListTodosService(SqlEventsDbContext context)
        {
            _context = context;
        }

        public IQueryable<TodoListDto> SortFilterPage(SqlSortFilterPageOptions options)
        {
            var todosQuery = _context.Todos            
                .AsNoTracking()
                .MapTodoEventsToDto()
                .OrderTodosEventsBy(options.OrderByOptions)  
                .FilterTodosEventsBy(options.FilterBy,       
                               options.FilterValue);   

            options.SetupRestOfDto(todosQuery);        

            return todosQuery.Page(options.PageNum-1,  
                                   options.PageSize);  
        }
    }

}