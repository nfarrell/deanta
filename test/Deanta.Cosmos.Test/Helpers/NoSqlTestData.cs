
using System;
using System.Collections.Generic;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;
using Deanta.Cosmos.DataLayer.EfClassesSql;

namespace Deanta.Cosmos.Test.Helpers
{
    public static class NoSqlTestData
    {
        public static TodoListNoSql CreateDummyNoSqlTodo(double votes = 0)
        {
            var todo = new TodoListNoSql
            {
                TodoId = Guid.NewGuid(),
                Title = "Test",
                CreatedAt = new DateTime(2000, 1, 1),
            };

            return todo;
        }

        public static List<TodoListNoSql> CreateDummyTodos(int numTodos = 10, bool stepByYears = false)
        {
            var random = new Random(0);
            var result = new List<TodoListNoSql>();
            var commonOwner = new Owner { Name = "CommonOwner" };
            for (var i = 0; i < numTodos; i++)
            {
                var createdDate = stepByYears
                    ? DddEfTestData.DummyTodoStartDate.AddYears(i)
                    : DddEfTestData.DummyTodoStartDate.AddDays(i);
                var todo = new TodoListNoSql
                {
                    TodoId = Guid.NewGuid(),
                    Title = $"Todo{i:D4} Title",
                    CreatedAt = createdDate,
                };
                result.Add(todo);
            }
            return result;
        }
    }
}