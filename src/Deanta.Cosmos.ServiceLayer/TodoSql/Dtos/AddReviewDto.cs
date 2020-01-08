using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    public class AddCommentDto : ILinkToEntity<Todo>
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