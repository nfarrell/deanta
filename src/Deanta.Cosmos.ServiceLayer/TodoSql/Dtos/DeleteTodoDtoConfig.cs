using System;
using System.Linq;
using AutoMapper;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using GenericServices.Configuration;

namespace Deanta.Cosmos.ServiceLayer.TodoSql.Dtos
{
    class DeleteTodoDtoConfig : PerDtoConfig<DeleteTodoDto, Todo>
    {
        public override Action<IMappingExpression<Todo, DeleteTodoDto>> AlterReadMapping
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