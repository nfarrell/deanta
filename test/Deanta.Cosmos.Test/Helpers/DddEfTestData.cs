
using System;
using System.Collections.Generic;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayer.EfCode;

namespace Deanta.Cosmos.Test.Helpers
{
    public static class DddEfTestData
    {
        public const string DummyUserId = "UnitTestUserId";
        public static readonly DateTime DummyTodoStartDate = new DateTime(2017, 1, 1);

        public static void SeedDatabaseDummyTodos(this DeantaSqlDbContext context, int numTodos = 10, bool stepByYears = false)
        {
            context.Todos.AddRange(CreateDummyTodos(numTodos, stepByYears));
            context.SaveChanges();
        }

        public static Todo CreateDummyTodoOneOwner()
        {
            var todo = Todo.CreateTodo
            ("Todo Title", new[] { new Owner { Name = "Test Owner" } }
            );

            return todo.Result;
        }

        public static Todo CreateDummyTodoTwoOwnersTwoComments()
        {
            var todo = Todo.CreateTodo
            (
                "Todo Title", new[] { new Owner { Name = "Owner1" }, new Owner { Name = "Owner2" } }
            );

            return todo.Result;
        }

        public static List<Todo> CreateDummyTodos(int numTodos = 10, bool stepByYears = false)
        {
            var result = new List<Todo>();
            var commonOwner = new Owner { Name = "CommonOwner" };
            for (var i = 0; i < numTodos; i++)
            {
                var todo = Todo.CreateTodo($"Todo{i:D4} Title", new[] { new Owner { Name = $"Owner{i:D4}" }, commonOwner }
                ).Result;

                result.Add(todo);
            }

            return result;
        }

        public static List<Todo> SeedDatabaseFourTodos(this DeantaSqlDbContext context)
        {
            var fourTodos = CreateFourTodos();
            context.Todos.AddRange(fourTodos);
            context.SaveChanges();
            return fourTodos;
        }

        public static List<Todo> CreateFourTodos()
        {
            var michaelHodgepodge = new Owner { Name = "Martin Fowler" };

            var todos = new List<Todo>();

            var todo1 = Todo.CreateTodo
            (
                "Buy Ham",
              new[] { new Owner { Name = "shbuuurt", Email = "nev@nev.ie" } }
            ).Result;
            todos.Add(todo1);

            var todo2 = Todo.CreateTodo
            (
                "Buy Cheese",
                new[] { new Owner() {Name = "Michael Fitz", Email = "neville@nev.ie"} }
            ).Result;
            todos.Add(todo2);

            var todo3 = Todo.CreateTodo
            (
                "Buy Bread",
               new[] { new Owner { Name = "Neville Farrell" } }
            ).Result;
            todos.Add(todo3);

            var todo4 = Todo.CreateTodo
            (
                "Buy milk",
                new[] { new Owner { Name = "Future Person" } }
            ).Result;
            todo4.AddComment("I look forward to reading this todo, if I am still alive!", "Jon P Smith");
            todo4.AddComment("I write this todo if I was still alive!", "Albert Einstein");

            todos.Add(todo4);

            return todos;
        }
    }
}