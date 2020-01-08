
using System;
using System.Collections.Generic;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.DataLayerEvents.EfCode;

namespace Deanta.Cosmos.Test.Helpers
{
    public static class WithEventsEfTestData
    {
        public const string DummyUserId = "UnitTestUserId";
        public static readonly DateTime DummyTodoStartDate = new DateTime(2017, 1, 1);

        public static void SeedDatabaseDummyTodos(this SqlEventsDbContext context, int numTodos = 10,
            bool stepByYears = false)
        {
            context.Todos.AddRange(CreateDummyTodos(numTodos, stepByYears));
            context.SaveChanges();
        }

        public static TodoWithEvents CreateDummyTodoOneOwner()
        {
            var todo = TodoWithEvents.CreateTodo
            (
                "Todo Title",
               new[] {new OwnerWithEvents("Test Owner", null)}
            );

            return todo.Result;
        }

        public static TodoWithEvents CreateDummyTodoTwoOwnersTwoComments()
        {
            var todo = TodoWithEvents.CreateTodo
            (
                "Todo Title",
             new[] {new OwnerWithEvents("Owner1", null), new OwnerWithEvents("Owner2", null)}
            );
           
            return todo.Result;
        }

        public static List<TodoWithEvents> CreateDummyTodos(int numTodos = 10, bool stepByYears = false)
        {
            var result = new List<TodoWithEvents>();
            var commonOwner = new OwnerWithEvents("CommonOwner", null);
            for (var i = 0; i < numTodos; i++)
            {
                var todo = TodoWithEvents.CreateTodo
                (
                    $"Todo{i:D4} Title",
                    new[] {new OwnerWithEvents($"Owner{i:D4}", null), commonOwner}
                ).Result;
                
                result.Add(todo);
            }

            return result;
        }

        public static List<TodoWithEvents> SeedDatabaseFourTodos(this SqlEventsDbContext context)
        {
            var fourTodos = CreateFourTodos();
            context.Todos.AddRange(fourTodos);
            context.SaveChanges();
            return fourTodos;
        }

        public static List<TodoWithEvents> CreateFourTodos()
        {
            var michaelHodgepodge = new OwnerWithEvents("Martin Fowler", null);

            var todos = new List<TodoWithEvents>();

            var todo1 = TodoWithEvents.CreateTodo
            (
                "Buy Milk",
              new[] {michaelHodgepodge}
            ).Result;
            todos.Add(todo1);

            var todo2 = TodoWithEvents.CreateTodo
            (
                "Collect Groceries",
                new[] {michaelHodgepodge}
            ).Result;
            todos.Add(todo2);

            var todo3 = TodoWithEvents.CreateTodo
            (
                "Go Shopping",
               new[] {michaelHodgepodge}
            ).Result;
            todos.Add(todo3);

            var todo4 = TodoWithEvents.CreateTodo
            (
                "Pickup Kids from School",
                new[] {new OwnerWithEvents("Future Person", null)}
            ).Result;
            
            todos.Add(todo4);

            return todos;
        }
    }
}