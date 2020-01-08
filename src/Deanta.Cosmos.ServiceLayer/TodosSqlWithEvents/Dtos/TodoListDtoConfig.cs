
using System;
using System.Linq;
using AutoMapper;
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;
using GenericServices.Configuration;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Dtos
{
    class TodoListDtoConfig : PerDtoConfig<TodoListDto, TodoWithEvents>
    {
        public override Action<IMappingExpression<TodoWithEvents, TodoListDto>> AlterReadMapping
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