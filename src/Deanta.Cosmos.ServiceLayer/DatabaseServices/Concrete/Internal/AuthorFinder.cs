
using System.Collections.Generic;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayer.EfCode;

namespace Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete.Internal
{
    internal class OwnerFinder
    {
        private readonly Dictionary<string, Owner> _authorDict;
        private DeantaSqlDbContext _context;

        public OwnerFinder(DeantaSqlDbContext context)
        {
            _context = context;
            _authorDict = context.Owners.ToDictionary(x => x.Name);
        }

        public IEnumerable<Owner> GetOwnersOfThisTodo(string owners)
        {
            foreach (var authorName in ExtractOwnersFromTodoData(owners))
            {
                if (!_authorDict.ContainsKey(authorName))
                {
                    _authorDict[authorName] = new Owner { Name = authorName };
                }

                yield return _authorDict[authorName];
            }
        }

        private static IEnumerable<string> ExtractOwnersFromTodoData(string owners)
        {
            return owners.Replace(" and ", ",").Replace(" with ", ",")
                .Split(',').Select(x => x.Trim()).Where(x => x.Length > 1);
        }
    }
}