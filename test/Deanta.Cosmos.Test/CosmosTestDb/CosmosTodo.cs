
using System;
using System.Collections.Generic;

namespace Deanta.Cosmos.Test.CosmosTestDb
{
    public class CosmosTodo
    {
        public int CosmosTodoId { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate { get; set; }

        //Used for checking on null handling
        public int? NullableInt { get; set; }

        //----------------------------------
        //relationships 

        public ICollection<CosmosComment> Comments { get; set; }
    }
}