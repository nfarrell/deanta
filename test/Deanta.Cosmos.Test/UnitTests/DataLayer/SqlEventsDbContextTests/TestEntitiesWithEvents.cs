
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.Infrastructure.EventHandlers;
using Deanta.Cosmos.Test.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.SqlEventsDbContextTests
{
    public class TestEntitiesWithEvents
    {
        private readonly ITestOutputHelper _output;

        public TestEntitiesWithEvents(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateTodoWithCommentsAndCheckCommentAddedHandlerOk()
        {
            
            var showLog = false;
            var options =
                SqliteInMemory.CreateOptionsWithLogging<SqlEventsDbContext>(x =>
                {
                    if (showLog)
                        _output.WriteLine(x.DecodeMessage());
                });
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
            {
                context.Database.EnsureCreated();

                
                showLog = true;
                var todo = WithEventsEfTestData.CreateDummyTodoTwoOwnersTwoComments();
                context.Add(todo);
                context.SaveChanges();

                
                todo.CommentsCount.ShouldEqual(2);
                todo.CommentsAverageVotes.ShouldEqual(6.0/2);
            }
        }

        [Fact]
        public void TestAddCommentToCreatedTodoAndCheckCommentAddedHandlerOk()
        {
            
            var showLog = false;
            var options =
                SqliteInMemory.CreateOptionsWithLogging<SqlEventsDbContext>(x =>
                {
                    if (showLog)
                        _output.WriteLine(x.DecodeMessage());
                });
            var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
            context.Database.EnsureCreated();
            var todo = WithEventsEfTestData.CreateDummyTodoOneOwner();
            context.Add(todo);
            context.SaveChanges();

            
            showLog = true;
            todo.AddComment(4, "OK", "me");
            context.SaveChanges();

            
            todo.CommentsCount.ShouldEqual(1);
            todo.CommentsAverageVotes.ShouldEqual(4);
        }

        [Fact]
        public void TestAddCommentToExistingTodoAndCheckCommentAddedHandlerOk()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            {
                var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
                context.Database.EnsureCreated();
                var todo = WithEventsEfTestData.CreateDummyTodoOneOwner();
                context.Add(todo);
                context.SaveChanges();
            }
            {
                var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
                var todo = context.Todos.Single();

                
                todo.AddComment(4, "OK", "me", context);
                context.SaveChanges();

                
                todo.CommentsCount.ShouldEqual(1);
                todo.CommentsAverageVotes.ShouldEqual(4);
            }
        }

        [Fact]
        public void TestCreateTodoWithCommentsThenRemoveCommentCheckCommentRemovedHandlerOk()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            {
                var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>(); 
                context.Database.EnsureCreated();
                var todo = WithEventsEfTestData.CreateDummyTodoTwoOwnersTwoComments();
                context.Add(todo);
                context.SaveChanges();

                
                var commentToRemove = todo.Comments.First();
                todo.RemoveComment(commentToRemove.CommentId);
                context.SaveChanges();

                
                todo.CommentsCount.ShouldEqual(1);
                todo.CommentsAverageVotes.ShouldEqual(1);
            }
        }

        //-------------------------------------------------------
        //OwnersOrdered events

        [Fact]
        public void TestCreateTodoWithOneOwnerAndChangeTheOwnersName()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            {
                var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
                context.Database.EnsureCreated();
                var todo = WithEventsEfTestData.CreateDummyTodoOneOwner();
                context.Add(todo);
                context.SaveChanges();

                
                todo.OwnersOrdered.ShouldEqual("Test Owner");
                todo.OwnersLink.First().Owner.ChangeName("New name");
                context.SaveChanges();

                
                context.Todos.First().OwnersOrdered.ShouldEqual("New name");
            }
        }

        [Fact]
        public void TestCreateTodoTwoOwnersChangeCommonOwnerOk()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            {
                var context = options.CreateDbWithDiForHandlers<SqlEventsDbContext, CommentAddedHandler>();
                context.Database.EnsureCreated();
                var todos = WithEventsEfTestData.CreateDummyTodos(2);
                context.AddRange(todos);
                context.SaveChanges();

                
                todos.First().OwnersOrdered.ShouldEqual("Owner0000, CommonOwner");
                todos.First().OwnersLink.Last().Owner.ChangeName("New common name");
                context.SaveChanges();

                
                var readTodos = context.Todos.ToList();
                readTodos.First().OwnersOrdered.ShouldEqual("Owner0000, New common name");
                readTodos.Last().OwnersOrdered.ShouldEqual("Owner0001, New common name");
            }
        }
    }
}