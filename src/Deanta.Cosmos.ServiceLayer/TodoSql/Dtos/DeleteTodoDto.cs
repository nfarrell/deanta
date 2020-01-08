using System;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using GenericServices;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    public class DeleteTodoDto : ILinkToEntity<Todo>
    {
        public Guid TodoId{ get; set; }
        public string Title { get; set; }
        public string OwnersOrdered { get; set; }
    }
}