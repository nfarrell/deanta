using System;
using System.Linq;
using AutoMapper;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using GenericServices.Configuration;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    class TodoListDtoConfig : PerDtoConfig<TodoListDto, Todo>
    {
        public override Action<IMappingExpression<Todo, TodoListDto>> AlterReadMapping
        {
            get
            {
                return cfg => cfg
                   .ForMember(x => x.OwnersOrdered, y => y.MapFrom(p => string.Join(", ",
                        p.OwnersLink.OrderBy(q => q.Order).Select(q => q.Owner.Name).ToList())));
            }
        }
    }
}