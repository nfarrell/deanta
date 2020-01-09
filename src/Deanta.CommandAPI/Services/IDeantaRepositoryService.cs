using System;
using System.Threading.Tasks;
using Deanta.Models.Contracts;


namespace Deanta.CommandAPI.Services
{
    public interface IDeantaRepositoryService
    {
        Task CreateTemplate(TodoDto newTodoModel, Guid userId);
    }
}
