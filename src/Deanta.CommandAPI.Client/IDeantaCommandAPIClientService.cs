using System;
using System.Threading.Tasks;
using Deanta.Models.Contracts;
using Deanta.Models.Enums;


namespace Deanta.CommandAPI.Client
{
    public interface IDeantaCommandApiClientService
    {
        Task CreateTodo(TodoDto newTask, Guid userId);

        Task UpdateTodo(Guid id, TodoDto newTemplate, Guid userId);

    }
}
