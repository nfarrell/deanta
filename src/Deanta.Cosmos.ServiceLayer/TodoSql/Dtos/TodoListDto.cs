using System;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using GenericServices;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    public class TodoListDto : ILinkToEntity<Todo>
    {
        public Guid TodoId { get; set; }

        public string Title { get; set; }

        public string OwnersOrdered { get; set; }

        //debatable whether this should be nullable or not, but oh well
        public DateTime CreatedAt { get; set; }
    }
}