using System;
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.DataLayerEvents.QueryExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Deanta.Cosmos.Infrastructure.ConcurrencyHandlers
{
    public class FixConcurrencyMethods
    {
        private readonly EntityEntry _entry;
        private readonly DbContext _context;

        public FixConcurrencyMethods(EntityEntry entry, DbContext context)
        {
            _entry = entry;
            _context = context;
        }
    }
}