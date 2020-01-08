
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.DomainEvents;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.DataLayer.SqlEventsDbContextTests
{
    public class TestEntitiesNoEvents
    {
        private readonly ITestOutputHelper _output;

        public TestEntitiesNoEvents(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateDatabaseAndSeedOk()
        {
            
            var options = SqliteInMemory.CreateOptionsWithLogging<SqlEventsDbContext>(x => _output.WriteLine(x.Message));
            using var context = new SqlEventsDbContext(options, null);
            context.Database.EnsureCreated();

            
            context.SeedDatabaseFourTodos();

            
            context.Todos.Count().ShouldEqual(4);
        }

        [Fact]
        public void TestCreateTodoAndCheckPartsOk()
        {
            
            var options = SqliteInMemory.CreateOptionsWithLogging<SqlEventsDbContext>(x => _output.WriteLine(x.Message));
            using (var context = new SqlEventsDbContext(options, null))
            {
                context.Database.EnsureCreated();
                var todo = WithEventsEfTestData.CreateDummyTodoTwoOwnersTwoComments();
                context.Add(todo);
                context.SaveChanges();
            }
            using (var context = new SqlEventsDbContext(options, null))
            {
                
                var todoWithRelationships = context.Todos
                    .Include(p => p.OwnersLink).ThenInclude(p => p.Owner)
                    .Include(p => p.Comments)
                    .Single();

                
                todoWithRelationships.OwnersLink.Select(y => y.Owner.Name).OrderBy(x => x).ToArray()
                    .ShouldEqual(new[]{ "Owner1" , "Owner2" });
                todoWithRelationships.Comments.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestTodoAddCommentCausesEventOk()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            using var context = new SqlEventsDbContext(options, null);
            context.Database.EnsureCreated();
            var todo = WithEventsEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            context.Add(todo);
            context.SaveChanges();
            todo.GetBeforeSaveEventsThenClear();

            
            todo.AddComment(5, "test", "someone");

            
            var dEvent = todo.GetBeforeSaveEventsThenClear().Single();
            dEvent.ShouldBeType<TodoCommentAddedEvent>();
        }

        [Fact]
        public void TestTodoRemoveCommentCausesEventOk()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            using var context = new SqlEventsDbContext(options, null);
            context.Database.EnsureCreated();
            var todo = WithEventsEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            context.Add(todo);
            context.SaveChanges();
            todo.GetBeforeSaveEventsThenClear();

            
            todo.RemoveComment(todo.Comments.First().CommentId);

            
            var dEvent = todo.GetBeforeSaveEventsThenClear().Single();
            dEvent.ShouldBeType<TodoCommentRemovedEvent>();
        }

        //---------------------------------------------------
        //Now the OwnersOrdered concurrency handling

        [Fact]
        public void TestOwnerChangeNameCausesEventOk()
        {
            
            var options = SqliteInMemory.CreateOptions<SqlEventsDbContext>();
            using var context = new SqlEventsDbContext(options, null);
            context.Database.EnsureCreated();
            var todo = WithEventsEfTestData.CreateDummyTodoTwoOwnersTwoComments();
            context.Add(todo);
            context.SaveChanges();
            var author = todo.OwnersLink.First().Owner;
            author.GetBeforeSaveEventsThenClear();

            
            author.ChangeName("new name");

            
            var dEvent = author.GetBeforeSaveEventsThenClear().Single();
            dEvent.ShouldBeType<OwnerNameUpdatedEvent>();
        }

    }
}