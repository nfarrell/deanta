using System;
using System.Linq;
using AutoFixture;
using Moq;
using Deanta.Models.Contexts;


namespace Deanta.UnitTests.Common
{
    public class FakeDeantaContextFactory
    {
        public static IDeantaContext CreateFakeDeantaContext()
        {
            var fixture = new Fixture();
            var index = 1;

            // To query our database we need to implement IQueryable 
            //var mockSet = MockTools.GetQueryableMockDbSet<TodoModel>(ToArray());

            var mockContext = new Mock<IDeantaContext>();

            //todo: Test won't run until this is setup
            //mockContext.Setup(c => c.Deantas).Returns(mockSet);

            return mockContext.Object;
        }
    }
}
