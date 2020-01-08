using System.Linq;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.Infrastructure.ConcurrencyHandlers;
using Deanta.Cosmos.Infrastructure.EventHandlers;
using Deanta.Cosmos.Test.Helpers;
using GenericEventRunner.ForSetup;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.SqlEventsDbContextTests
{
    public class TestEntitiesWithEventsConcurrency
    {
        private readonly ITestOutputHelper _output;

        public TestEntitiesWithEventsConcurrency(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddCommentCauseConcurrencyThrown()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
            context.Database.EnsureCreated();
            var todo = WithEventsEfTestData.CreateDummyTodoOneOwner();
            context.Add(todo);
            context.SaveChanges();

            
            todo.AddComment(4, "OK", "me");
            //This simulates adding a comment with NumStars of 2 before the AddComment 
            context.Database.ExecuteSqlRaw(
                "UPDATE Todos SET CommentsCount = @p0, CommentsAverageVotes = @p1 WHERE TodoId = @p2",
                1, 2, todo.TodoId);

            
            Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
        }

        [Fact]
        public void TestAddCommentConcurrencyFixed()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            var config = new GenericEventRunnerConfig
            {
                SaveChangesExceptionHandler = TodoWithEventsConcurrencyHandler.HandleCacheValuesConcurrency
            };
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>(config: config);
            context.Database.EnsureCreated();
            var todo = WithEventsEfTestData.CreateDummyTodoOneOwner();
            context.Add(todo);
            context.SaveChanges();

            todo.AddComment(4, "OK", "me");
            //This simulates adding a comment with NumStars of 2 before the AddComment 
            context.Database.ExecuteSqlRaw(
                "UPDATE Todos SET CommentsCount = @p0, CommentsAverageVotes = @p1 WHERE TodoId = @p2",
                1, 2, todo.TodoId);

            
            context.SaveChanges();

            
            var foundTodo = context.Find<TodoWithEvents>(todo.TodoId);
            foundTodo.CommentsCount.ShouldEqual(2);
            foundTodo.CommentsAverageVotes.ShouldEqual(6.0/2.0);
        }

        [Fact]
        public void TestAddSaveChangesExceptionHandlerButStillFailsOnOtherDbExceptions()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            var config = new GenericEventRunnerConfig
            {
                SaveChangesExceptionHandler = TodoWithEventsConcurrencyHandler.HandleCacheValuesConcurrency
            };
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>(config: config);
            context.Database.EnsureCreated();

            var comment = new CommentWithEvents(1,"hello", "Me");
            context.Add(comment);

            
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            
            ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
        }

        //---------------------------------------------------
        //OwnersOrdered concurrency handling

        [Fact]
        public void TestChangeOwnerOrderedCauseConcurrencyThrown()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
            context.Database.EnsureCreated();
            var todos = WithEventsEfTestData.CreateDummyTodos(2);
            context.AddRange(todos);
            context.SaveChanges();

            
            todos.First().OwnersLink.Last().Owner.ChangeName("New common name");
            //This simulates changing the OwnersOrdered value
            context.Database.ExecuteSqlRaw(
                "UPDATE Todos SET OwnersOrdered = @p0 WHERE TodoId = @p1",
                "different author string", todos.First().TodoId);

            
            Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
        }

        [Fact]
        public void TestChangeOwnerOrderedConcurrencyFixed()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            var config = new GenericEventRunnerConfig
            {
                SaveChangesExceptionHandler = TodoWithEventsConcurrencyHandler.HandleCacheValuesConcurrency
            };
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>(config: config);
            context.Database.EnsureCreated();
            var todos = WithEventsEfTestData.CreateDummyTodos(2);
            context.AddRange(todos);
            context.SaveChanges();

            
            todos.First().OwnersLink.Last().Owner.ChangeName("New common name");
            //This simulates changing the OwnersOrdered value
            context.Database.ExecuteSqlRaw(
                "UPDATE Todos SET OwnersOrdered = @p0 WHERE TodoId = @p1",
                "different author string", todos.First().TodoId);

            
            context.SaveChanges();

            
            var readTodos = context.Todos.ToList();
            readTodos.First().OwnersOrdered.ShouldEqual("Owner0000, New common name");
            readTodos.Last().OwnersOrdered.ShouldEqual("Owner0001, New common name");
        }

        [Fact]
        public void TestTodoDeletedConcurrencyFixed()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            var config = new GenericEventRunnerConfig
            {
                SaveChangesExceptionHandler = TodoWithEventsConcurrencyHandler.HandleCacheValuesConcurrency
            };
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>(config: config);
            context.Database.EnsureCreated();
            var todos = WithEventsEfTestData.CreateDummyTodos(2);
            context.AddRange(todos);
            context.SaveChanges();

            
            todos.First().OwnersLink.Last().Owner.ChangeName("New common name");
            //This simulates changing the OwnersOrdered value
            context.Database.ExecuteSqlRaw("DELETE FROM Todos WHERE TodoId = @p0", todos.First().TodoId);

            
            context.SaveChanges();

            
            var readTodos = context.Todos.ToList();
            readTodos.Count.ShouldEqual(1);
            readTodos.First().OwnersOrdered.ShouldEqual("Owner0001, New common name");
        }
    }
}