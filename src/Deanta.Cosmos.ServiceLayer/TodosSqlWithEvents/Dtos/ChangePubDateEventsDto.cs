
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Dtos
{
    public class ChangePubDateEventsDto : ILinkToEntity<TodoWithEvents>
    {
        [HiddenInput]
        public Guid TodoId{ get; set; }

        [ReadOnly(true)]
        public string Title { get; set; }

        [DataType(DataType.Date)]               
        public DateTime CreatedAt { get; set; }
    }
}