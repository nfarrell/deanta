using System;
using System.Collections.Generic;
using System.Linq;
using Deanta.Models.Contexts;
using Deanta.Models.Models;
using Deanta.Models.Paging;
using Microsoft.Extensions.Primitives;

namespace Deanta.QueryAPI.Services
{
    public class DeantaRepositoryService : IDeantaRepositoryService
    {
        private readonly IDeantaContext _deantaContext;

        public DeantaRepositoryService(IDeantaContext deantaContext)
        {
            _deantaContext = deantaContext;
        }

        public TodoModel GetTodo(Guid id)
        {
            return _deantaContext.Todos.FirstOrDefault(x => x.TodoId == id);
        }

        public PagedList<TodoModel> FetchTasks(bool isCompleted, int? page = null, int? pageSize = null,
            Dictionary<string, StringValues> parsedQuery = null)
        {
            var tasks = _deantaContext.Todos.Where(x => x.IsCompleted == isCompleted).AsQueryable();

            if (parsedQuery?.ContainsKey("search_string") ?? false)
            {
                tasks = tasks.Where(r => r.Title.Contains(parsedQuery["search_string"])).AsQueryable();
            }

            var order = string.Empty;
            if (parsedQuery?.ContainsKey("order") ?? false)
            {
                order = parsedQuery["order"];
            }

            switch (order)
            {
                case "name_desc":
                    tasks = tasks.OrderByDescending(s => s.Title).AsQueryable();
                    break;

            }

            return new PagedList<TodoModel>(tasks, page ?? 1, pageSize ?? 10);
        }
    }
}
