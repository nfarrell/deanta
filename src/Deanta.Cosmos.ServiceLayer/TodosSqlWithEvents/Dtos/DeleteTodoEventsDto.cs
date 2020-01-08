
using System;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using GenericServices;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Dtos
{
    public class DeleteTodoEventsDto : ILinkToEntity<TodoWithEvents>
    {
        public Guid TodoId{ get; set; }
        public string Title { get; set; }
        public string OwnersOrdered { get; set; }
    }
}