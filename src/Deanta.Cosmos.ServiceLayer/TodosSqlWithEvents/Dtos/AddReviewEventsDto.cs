
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Dtos
{
    public class AddCommentEventsDto : ILinkToEntity<TodoWithEvents>
    {
        [HiddenInput]
        public Guid TodoId{ get; set; }

        [ReadOnly(true)]
        public string Title { get; set; }

        public string CommentOwner { get; set; }

        public int NumStars { get; set; }
        public string Comment { get; set; }
    }
}