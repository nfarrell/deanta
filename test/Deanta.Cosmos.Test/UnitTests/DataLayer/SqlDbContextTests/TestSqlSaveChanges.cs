
using System.Linq;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.NoSqlCode;
using Deanta.Cosmos.Test.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.DeantaSqlDbContextTests
{
    public class TestSqlSaveChanges
    {
        private DbContextOptions<DeantaSqlDbContext> _sqlOptions;
        public TestSqlSaveChanges()
        {

            _sqlOptions = this.CreateUniqueClassOptions<DeantaSqlDbContext>();
            using var context = new DeantaSqlDbContext(_sqlOptions);
            context.Database.EnsureCreated();
            context.WipeAllDataFromDatabase();
        }

        [Fact]
        public void TestSaveChangesAddNoSqlOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
            sqlContext.Database.EnsureCreated();
            noSqlContext.Database.EnsureCreated();

            
            var todo = DddEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            sqlContext.Add(todo);
            sqlContext.SaveChanges();

            
            sqlContext.Todos.Count().ShouldEqual(1);
            var noSqlTodo = noSqlContext.Todos.SingleOrDefault(p => p.TodoId == todo.TodoId);
            noSqlTodo.ShouldNotBeNull();
            noSqlTodo.OwnersOrdered.ShouldEqual("Owner1, Owner2");
            noSqlTodo.CommentsCount.ShouldEqual(2);
        }

        [Fact]
        public void TestSaveChangesDeleteNoSqlOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
            sqlContext.Database.EnsureCreated();
            noSqlContext.Database.EnsureCreated();
            var todo = DddEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            sqlContext.Add(todo);
            sqlContext.SaveChanges();

            
            sqlContext.Remove(todo);
            sqlContext.SaveChanges();

            
            sqlContext.Todos.Count().ShouldEqual(0);
            var noSqlTodo = noSqlContext.Todos.SingleOrDefault(p => p.TodoId == todo.TodoId);
            noSqlTodo.ShouldBeNull();
        }

        [Fact]
        public void TestSaveChangesDirectUpdatesNoSqlOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            using (var noSqlContext = new NoDeantaSqlDbContext(builder.Options))
            {
                using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
                sqlContext.Database.EnsureCreated();
                noSqlContext.Database.EnsureCreated();
                var todo = DddEfTestData.CreateDummyTodoTwoOwnersTwoComments();
                sqlContext.Add(todo);
                sqlContext.SaveChanges();
            }

            using (var noSqlContext = new NoDeantaSqlDbContext(builder.Options))
            {
                using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
                
                var todo = sqlContext.Todos.Single();
                
                sqlContext.SaveChanges();

                
                sqlContext.Todos.Count().ShouldEqual(1);
                var noSqlTodo = noSqlContext.Todos.Single(p => p.TodoId == todo.TodoId);
                noSqlTodo.CreatedAt.ShouldEqual(DddEfTestData.DummyTodoStartDate.AddDays(1));

            }
        }

        [Fact]
        public void TestSaveChangesIndirectUpdatesNoSqlOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            using (var noSqlContext = new NoDeantaSqlDbContext(builder.Options))
            {
                using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
                sqlContext.Database.EnsureCreated();
                noSqlContext.Database.EnsureCreated();
                var todo = DddEfTestData.CreateDummyTodoTwoOwnersTwoComments();
                sqlContext.Add(todo);
                sqlContext.SaveChanges();
            }

            using (var noSqlContext = new NoDeantaSqlDbContext(builder.Options))
            {
                using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
                
                var todo = sqlContext.Todos.Single();
                todo.AddComment(5, "xxx","yyy", sqlContext);
                sqlContext.SaveChanges();

                
                sqlContext.Todos.Count().ShouldEqual(1);
                var noSqlTodo = noSqlContext.Todos.Single(p => p.TodoId == todo.TodoId);
                noSqlTodo.CommentsCount.ShouldEqual(3);
            }
        }

        [Fact]
        public void TestSaveChangesChangeOwnerTwoTodosNoSqlOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            using (var noSqlContext = new NoDeantaSqlDbContext(builder.Options))
            {
                using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
                sqlContext.Database.EnsureCreated();
                noSqlContext.Database.EnsureCreated();
                sqlContext.SeedDatabaseFourTodos();
            }

            using (var noSqlContext = new NoDeantaSqlDbContext(builder.Options))
            {
                using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
                
                var author = sqlContext.Owners.Single(x => x.Name == "Martin Fowler");
                var todoIds = sqlContext.TodoOwners
                    .Where(x => x.OwnerId == author.OwnerId)
                    .Select(x => x.TodoId).ToList();
                author.Name = "Different Name";
                sqlContext.SaveChanges();

                
                sqlContext.Todos.Count().ShouldEqual(4);
                var noSqlTodos = noSqlContext.Todos.Where(p => todoIds.Contains(p.TodoId)).ToList();
                noSqlTodos.Count.ShouldEqual(2);
                noSqlTodos.All(x => x.OwnersOrdered == "Different Name").ShouldBeTrue();
            }
        }

        [Fact]
        public void TestSaveChangesSoftDeleteNoSqlOk()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    GetType().Name);
            using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
            sqlContext.Database.EnsureCreated();
            noSqlContext.Database.EnsureCreated();
            var todo = DddEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            sqlContext.Add(todo);
            sqlContext.SaveChanges();

            
            todo.SoftDeleted = true;
            sqlContext.SaveChanges();

            
            sqlContext.Todos.Count().ShouldEqual(0);
            sqlContext.Todos.IgnoreQueryFilters().Count().ShouldEqual(1);
            var noSqlTodo = noSqlContext.Todos.SingleOrDefault(p => p.TodoId == todo.TodoId);
            noSqlTodo.ShouldBeNull();
        }

        [Fact]
        public void TestSaveChangesUpdatesNoSqlFail()
        {
            
            var config = AppSettings.GetConfiguration();
            var builder = new DbContextOptionsBuilder<NoDeantaSqlDbContext>()
                .UseCosmos(
                    config["endpoint"],
                    config["authKey"],
                    "UNKNOWNDATABASENAME");
            using var noSqlContext = new NoDeantaSqlDbContext(builder.Options);
            using var sqlContext = new DeantaSqlDbContext(_sqlOptions, new NoSqlTodoUpdater(noSqlContext));
            sqlContext.Database.EnsureCreated();

            
            var todo = DddEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            sqlContext.Add(todo);
            var ex = Assert.Throws<CosmosException> (() => sqlContext.SaveChanges());

            
            sqlContext.Todos.Count().ShouldEqual(0);
            ex.Message.ShouldStartWith("Response status code does not indicate success: 404 Substatus:");
        }
    }
}