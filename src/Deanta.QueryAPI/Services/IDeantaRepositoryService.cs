using Deanta.Models.Models;
using System;

namespace Deanta.QueryAPI.Services
{
    public interface IDeantaRepositoryService
    {
        TodoModel GetTodo(Guid id);
    }
}
