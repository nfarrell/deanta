using Deanta.Cosmos.DataLayer.EfClassesSql;
using System.Collections.Generic;
using System.Linq;

namespace Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete
{
    public static class SpecialTodo
    {
        public static Todo CreateSampleTodo()
        {
            var todo4 = Todo.CreateTodo("Quantum Networking", new List<Owner> { new Owner { Name = "David Lyons" } }).Result;

            todo4.AddComment("I look forward to reading this todo, if I am still alive!", "Jon P Smith");
            todo4.AddComment("I would write this todo if I was still alive!", "Albert Einstein");

            //special business logic for task here

            return todo4;
        }
    }
}