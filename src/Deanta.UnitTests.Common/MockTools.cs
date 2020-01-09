﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Deanta.UnitTests.Common
{
    public class MockTools
    {
        public static DbSet<T> GetQueryableMockDbSet<T>(params T[] sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.As<IQueryable>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet.Object;
        }
    }
}
