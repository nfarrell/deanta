using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    public class CreateTodoDto : ILinkToEntity<Todo>
    {
        public CreateTodoDto()
        {
            CreatedAt = DateTime.Today;
        }

        //This will be populated with the primary key of the created todo
        public Guid TodoId{ get; set; }

        //I would normally have the Required attribute to catch this at the front end
        //But to show how the static create method catches that error I have commented it out
        //[Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        public string Publisher { get; set; }

        [Range(0,1000)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Owner> Owners { get; set; }

        public List<KeyName> AllPossibleOwners { get; private set; }

        public List<int> TodoOwnerIds { get; set; } = new List<int>();

        public void BeforeDisplay(DbContext context)
        {
            AllPossibleOwners = context.Set<Owner>().Select(x => new KeyName(x.OwnerId, x.Name))
                .OrderBy(x => x.Name).ToList();
        }

        public void BeforeSave(DbContext context)
        {
            Owners = TodoOwnerIds.Select(x => context.Find<Owner>(x)).Where(x => x != null).ToList();
        }

        //---------------------------------------------------------
        //Now the data for the front end

        public struct KeyName
        {
            public KeyName(Guid authorId, string name)
            {
                OwnerId = authorId;
                Name = name;
            }

            public Guid OwnerId { get; }
            public string Name { get; }
        }
    }
}