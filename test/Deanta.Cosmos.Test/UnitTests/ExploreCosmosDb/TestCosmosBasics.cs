
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.Test.CosmosTestDb;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.ExploreCosmosDb
{
    public class TestCosmosBasics
    {
        private ITestOutputHelper _output;

        public TestCosmosBasics(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestAddCosmosTodoHaveToSetKeyOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using var context = new DeantaDbContext(options);
            await context.Database.EnsureCreatedAsync();


            var cTodo = new CosmosTodo { CosmosTodoId = 0, Title = "Todo" };
            var ex = Assert.Throws<NotSupportedException>(() => context.Add(cTodo));


            ex.Message.ShouldStartWith("The 'CosmosTodoId' on entity type 'CosmosTodo' does not have a value set and no value generator is available for properties of type 'int'.");
        }

        [Fact]
        public async Task TestAnyOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using var context = new DeantaDbContext(options);
            await context.Database.EnsureCreatedAsync();
            var cTodo1 = new CosmosTodo { CosmosTodoId = 1, Title = "Todo1" };
            var cTodo2 = new CosmosTodo { CosmosTodoId = 2, Title = "Todo2" };
            context.AddRange(cTodo1, cTodo2);


            var ex = await Assert.ThrowsAsync<CosmosException>(async () => await context.SaveChangesAsync());


            ex.Message.ShouldContain("Resource with specified id or name already exists.");
        }

        [Fact]
        public async Task TestAddCosmosTodoHowToUpdateOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using (var context = new DeantaDbContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var cTodo = new CosmosTodo { CosmosTodoId = 1, Title = "Todo1" };
                context.Add(cTodo);
                await context.SaveChangesAsync();

            }

            await using (var context = new DeantaDbContext(options))
            {

                var cTodo = await context.Todos.FindAsync(1); //You must read to get the "id"
                cTodo.Title = "Todo2";
                await context.SaveChangesAsync();
            }

            await using (var context = new DeantaDbContext(options))
            {

                context.Todos.Find(1).Title.ShouldEqual("Todo2");
            }
        }

        [Fact]
        public async Task TestNullableIntOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using (var context = new DeantaDbContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var cTodo1 = new CosmosTodo { CosmosTodoId = 1, NullableInt = null };
                var cTodo2 = new CosmosTodo { CosmosTodoId = 2, NullableInt = 1 };
                context.AddRange(cTodo1, cTodo2);
                await context.SaveChangesAsync();
            }

            await using (var context = new DeantaDbContext(options))
            {

                var cTodo1 = await context.Todos.FindAsync(1);
                var cTodo2 = await context.Todos.FindAsync(2);


                cTodo1.NullableInt.ShouldBeNull();
                cTodo2.NullableInt.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestNullableIntOrderByOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using (var context = new DeantaDbContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var cTodo1 = new CosmosTodo { CosmosTodoId = 1, NullableInt = null };
                var cTodo2 = new CosmosTodo { CosmosTodoId = 2, NullableInt = 1 };
                context.AddRange(cTodo1, cTodo2);
                await context.SaveChangesAsync();
            }

            await using (var context = new DeantaDbContext(options))
            {

                var ex = await Assert.ThrowsAsync<NotSupportedException>(async ()
                    => await context.Todos.OrderBy(x => x.NullableInt).ToListAsync());


                ex.Message.ShouldStartWith("Cannot execute cross partition order-by queries on mix types. " +
                                           "Consider using IS_STRING/IS_NUMBER to get around this exception.");
            }
        }

        [Fact]
        public async Task TestStringWithNullOrderByOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using (var context = new DeantaDbContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var cTodo1 = new CosmosTodo { CosmosTodoId = 1, };
                var cTodo2 = new CosmosTodo { CosmosTodoId = 2, Title = "Todo2" };
                context.AddRange(cTodo1, cTodo2);
                await context.SaveChangesAsync();
            }

            await using (var context = new DeantaDbContext(options))
            {

                var ex = await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await context.Todos.OrderBy(x => x.Title).ToListAsync());


                ex.Message.ShouldStartWith("Cannot execute cross partition order-by queries on mix types. " +
                                           "Consider using IS_STRING/IS_NUMBER to get around this exception.");
            }
        }

        [Fact]
        public async Task TestNullableIntWhereOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using (var context = new DeantaDbContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var cTodo1 = new CosmosTodo { CosmosTodoId = 1, NullableInt = null };
                var cTodo2 = new CosmosTodo { CosmosTodoId = 2, NullableInt = 1 };
                context.AddRange(cTodo1, cTodo2);
                await context.SaveChangesAsync();
            }

            await using (var context = new DeantaDbContext(options))
            {

                var todo = await context.Todos.SingleOrDefaultAsync(x => x.NullableInt > 0);


                todo.CosmosTodoId.ShouldEqual(2);
            }
        }

        [Fact]
        public async Task TestAddCosmosTodoAddSameKeyOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using (var context = new DeantaDbContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var cTodo = new CosmosTodo { CosmosTodoId = 1, Title = "Todo1" };
                context.Add(cTodo);
                await context.SaveChangesAsync();

            }

            await using (var context = new DeantaDbContext(options))
            {

                var cTodo = new CosmosTodo { CosmosTodoId = 1, Title = "Todo2" };
                context.Add(cTodo);
                context.Entry(cTodo).State.ShouldEqual(EntityState.Added);
                var ex = await Assert.ThrowsAsync<CosmosException>(async () => await context.SaveChangesAsync());


                ex.Message.ShouldContain("Resource with specified id or name already exists.");

            }

            await using (var context = new DeantaDbContext(options))
            {
                context.Todos.Find(1).Title.ShouldEqual("Todo1");
            }
        }

        [Fact]
        public async Task TestAddCosmosTodoWithCommentsOk()
        {

            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using var context = new DeantaDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();


            var cTodo = new CosmosTodo
            {
                CosmosTodoId = 1,      //NOTE: You have to provide a key value!
                Title = "Todo Title",
                CreatedDate = new DateTime(2019, 1, 1),
                Comments = new List<CosmosComment>
                {
                    new CosmosComment{Comment = "Great!", NumStars = 5, CommentOwner = "Commenter1"},
                    new CosmosComment{Comment = "Hated it", NumStars = 1, CommentOwner = "Commenter2"}
                }
            };
            context.Add(cTodo);
            await context.SaveChangesAsync();


            (await context.Todos.FindAsync(1)).Comments.Count.ShouldEqual(2);
        }

        [RunnableInDebugOnly]
        public async Task TestReadCurrentDbOk()
        {
            var options = this.GetCosmosDbToEmulatorOptions<DeantaDbContext>();
            await using var context = new DeantaDbContext(options);

            var todo = await context.Todos.FindAsync(2);

            //neville add tests here shbuuurt
        }
    }
}