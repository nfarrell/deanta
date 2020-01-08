
using System;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.NoSqlCode.Internal;
using Deanta.Cosmos.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.DeantaSqlDbContextTests
{
    public class TestFindTodoChanges
    {
        [Fact]
        public void TestAddTodoOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using var context = new DeantaSqlDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourTodos();

            
            context.Add(DddEfTestData.CreateDummyTodoOneOwner());
            var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

            
            changes.Single().TodoId.ShouldNotEqual(Guid.Empty);
            changes.Single().State.ShouldEqual(EntityState.Added);
        }

        [Fact]
        public void TestDeleteTodoOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using var context = new DeantaSqlDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourTodos();

            
            context.Remove(context.Todos.First());
            var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

            
            changes.Single().TodoId.ShouldNotEqual(Guid.Empty);
            changes.Single().State.ShouldEqual(EntityState.Deleted);
        }

        [Fact]
        public void TestSoftDeleteTodoOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using var context = new DeantaSqlDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourTodos();

            
            var todo = context.Todos.First();
            todo.SoftDeleted = true;
            var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

            
            changes.Single().TodoId.ShouldNotEqual(Guid.Empty);
            changes.Single().State.ShouldEqual(EntityState.Deleted);
        }

        [Fact]
        public void TestUpdateDirectOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using (var context = new DeantaSqlDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourTodos();
            }
            using (var context = new DeantaSqlDbContext(options))
            {
                
                var todo = context.Todos.First();
                var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

                
                changes.Single().TodoId.ShouldNotEqual(Guid.Empty);
                changes.Single().State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestUpdateViaNonTodoEntityOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using (var context = new DeantaSqlDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourTodos();
            }
            using (var context = new DeantaSqlDbContext(options))
            {
                var todo = context.Todos.First();
                var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

                changes.Single().TodoId.ShouldNotEqual(Guid.Empty);
                changes.Single().State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestUpdateViaOwnerChangeOneTodoOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using (var context = new DeantaSqlDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourTodos();
            }
            using (var context = new DeantaSqlDbContext(options))
            {
                
                var author = context.Owners.Single(x => x.Name == "Future Person");
                author.Name = "Different Name";
                var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

                
                changes.Single().TodoId.ShouldNotEqual(Guid.Empty);
                changes.Single().State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestUpdateViaOwnerChangeTwoTodosOk()
        {
            
            var options = SqliteInMemory.CreateOptions<DeantaSqlDbContext>();
            using (var context = new DeantaSqlDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourTodos();
            }
            using (var context = new DeantaSqlDbContext(options))
            {
                
                var author = context.Owners.Single(x => x.Name == "Martin Fowler");
                author.Name = "Different Name";
                var changes = TodoChangeInfo.FindTodoChanges(context.ChangeTracker.Entries().ToList(), context);

                
                changes.Count.ShouldEqual(2);
                changes.All(x => x.State == EntityState.Modified).ShouldBeTrue();
            }
        }
    }
}