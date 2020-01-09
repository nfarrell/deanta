using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deanta.Models.Contracts;
using Deanta.Models.Paging;

namespace Deanta.QueryAPI.Client
{
    public interface IDeantaQueryApiClientService
    {
        Task<TodoDto> GetTodo(Guid id);

        Task<PagedList<TodoDto>> GetTodos(int? page = null, int? pageSize = null,
            Dictionary<string, string> queryParameters = null);
    }
}
