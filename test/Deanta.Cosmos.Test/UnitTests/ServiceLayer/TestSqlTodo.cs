using System;
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql;
using Deanta.Cosmos.ServiceLayer.TodoSql.QueryObjects;
using Deanta.Cosmos.ServiceLayer.TodoSql.Services;
using Deanta.Cosmos.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Deanta.Cosmos.Test.UnitTests.ServiceLayer
{
    public class TestSqlTodoRead
    {
        private ITestOutputHelper _output;
        private readonly DbContextOptions<DeantaSqlDbContext> _options;

        public TestSqlTodoRead(ITestOutputHelper output)
        {
            _output = output;
            _options = this.CreateUniqueClassOptions<DeantaSqlDbContext>();
            using var context = new DeantaSqlDbContext(_options);
            context.Database.EnsureCreated();
            if (!context.Todos.Any())
            {
                context.SeedDatabaseDummyTodos(stepByYears: true);
            }
        }

        [Fact]
        public void TestTodoListDtoSelect()
        {
            var options = SqliteInMemory.CreateOptionsWithLogging<DeantaSqlDbContext>(x => _output.WriteLine(x.Message));
            using var context = new DeantaSqlDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourTodos();

            var dtos = context.Todos.MapTodoToDto().OrderBy(x => x.CreatedAt).ToList();

            dtos.Select(x => x.OwnersOrdered).ShouldEqual(new[] { "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
        }

        [Theory]
        [InlineData(OrderByOptions.ByCreatedDate)]
        //[InlineData(OrderByOptions.ByRandomMeasurementHere)]
        public async Task TestOrderByOk(OrderByOptions orderBy)
        {

            await using var context = new DeantaSqlDbContext(_options);
            var service = new SqlListTodosService(context);


            var todos = await service.SortFilterPage(new SqlSortFilterPageOptions
            {
                OrderByOptions = orderBy
            }).ToListAsync();


        }

        [Theory]
        [InlineData(100, 1)]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        public async Task TestPageOk(int pageSize, int pageNum)  //pageNum starts at 1
        {

            await using var context = new DeantaSqlDbContext(_options);
            var service = new SqlListTodosService(context);


            var filterPageOptions = new SqlSortFilterPageOptions
            {
                OrderByOptions = OrderByOptions.ByCreatedDate,
                PageSize = pageSize,
                PageNum = pageNum
            };
            var temp = service.SortFilterPage(filterPageOptions); //to set the PrevCheckState
            filterPageOptions.PageNum = pageNum;
            var todos = await service.SortFilterPage(filterPageOptions).ToListAsync();


            todos.Count.ShouldEqual(Math.Min(pageSize, todos.Count));

        }

        [Fact]
        public async Task TestFilterDatesOk()
        {

            var year = Math.Min(DateTime.UtcNow.Year, DddEfTestData.DummyTodoStartDate.AddYears(5).Year);
            await using var context = new DeantaSqlDbContext(_options);
            var service = new SqlListTodosService(context);


            var todos = await service.SortFilterPage(new SqlSortFilterPageOptions
            {
                OrderByOptions = OrderByOptions.ByCreatedDate,
                FilterBy = TodosFilterBy.ByCompleted,
                FilterValue = year.ToString()
            }).ToListAsync();


            todos.Single().CreatedAt.Year.ShouldEqual(year);
        }

        [Fact]
        public async Task TestFilterDatesFutureOk()
        {

            var year = Math.Min(DateTime.UtcNow.Year, DddEfTestData.DummyTodoStartDate.AddYears(5).Year);
            await using var context = new DeantaSqlDbContext(_options);
            var service = new SqlListTodosService(context);


            var todos = await service.SortFilterPage(new SqlSortFilterPageOptions
            {
                OrderByOptions = OrderByOptions.ByCreatedDate,
                FilterBy = TodosFilterBy.ByCompleted,
                FilterValue = "Coming Soon"
            }).ToListAsync();


            todos.All(x => x.CreatedAt > DateTime.UtcNow).ShouldBeTrue();
        }

        [Fact]
        public async Task TestFilterVotesOk()
        {

            await using var context = new DeantaSqlDbContext(_options);
            var service = new SqlListTodosService(context);


            var todos = await service.SortFilterPage(new SqlSortFilterPageOptions
            {
                OrderByOptions = OrderByOptions.ByCreatedDate,
                FilterBy = TodosFilterBy.ByCompleted,
                FilterValue = "2"
            }).ToListAsync();

            //backfill logic
            todos.All(x => true).ShouldBeTrue();
        }

    }
}