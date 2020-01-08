
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Cosmos.DataLayer.EfClassesSql
{
    public class Owner  : IOwnerId
    {
        //todo: move to global constants file
        public const int NameLength = 100;
        public const int EmailLength = 100;

        public Owner() { }

        public Guid OwnerId { get;  set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(NameLength)]
        public string Name { get;  set; }

        [MaxLength(EmailLength)]
        public string Email { get; set; }
        
        public ICollection<TodoOwner> TodosLink { get; set; }
    }

}