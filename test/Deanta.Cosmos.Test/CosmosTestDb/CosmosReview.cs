
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.Test.CosmosTestDb
{
    [Owned]
    public class CosmosComment
    {
        public string CommentOwner { get; set; }
        public int NumStars { get; set; }
        public string Comment { get; set; }
    }
}