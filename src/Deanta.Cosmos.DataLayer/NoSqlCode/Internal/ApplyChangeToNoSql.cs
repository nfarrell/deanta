using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Deanta.Cosmos.DataLayer.EfClassesNoSql;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Deanta.Cosmos.Test")]

namespace Deanta.Cosmos.DataLayer.NoSqlCode.Internal
{
    internal class ApplyChangeToNoSql
    {
        private static readonly MapperConfiguration SqlToNoSqlMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Todo, TodoListNoSql>();
            //.ForMember(p => p.DateCreated,
            //    m => m.MapFrom(s => s.DateCreated))
            //special task list business logic in here...

            cfg.CreateMap<TodoListNoSql, TodoListNoSql>();
        });

        private readonly NoDeantaSqlDbContext _noSqlContext;
        private readonly DbContext _sqlContext;

        public ApplyChangeToNoSql(DbContext sqlContext, NoDeantaSqlDbContext noSqlContext)
        {
            _sqlContext = sqlContext ?? throw new ArgumentNullException(nameof(sqlContext));
            _noSqlContext = noSqlContext ?? throw new ArgumentNullException(nameof(noSqlContext)); ;
        }

        public bool UpdateNoSql(IImmutableList<TodoChangeInfo> todosToUpdate)
        {
            if (_noSqlContext == null || !todosToUpdate.Any()) return false;

            foreach (var todoToUpdate in todosToUpdate)
            {
                switch (todoToUpdate.State)
                {
                    case EntityState.Deleted:
                        {
                            var noSqlTodo = _noSqlContext.Find<TodoListNoSql>(todoToUpdate.TodoId);
                            _noSqlContext.Remove(noSqlTodo);
                            break;
                        }
                    case EntityState.Modified:
                        {
                            var noSqlTodo = _noSqlContext.Find<TodoListNoSql>(todoToUpdate.TodoId);
                            var update = _sqlContext.Set<Todo>()
                                .ProjectTo<TodoListNoSql>(SqlToNoSqlMapper)
                                .Single(x => x.TodoId == todoToUpdate.TodoId);
                            SqlToNoSqlMapper.CreateMapper().Map(update, noSqlTodo);
                            break;
                        }
                    case EntityState.Added:
                        var newTodo = _sqlContext.Set<Todo>()
                            .ProjectTo<TodoListNoSql>(SqlToNoSqlMapper)
                            .Single(x => x.TodoId == todoToUpdate.TodoId);
                        _noSqlContext.Add(newTodo);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        public async Task<bool> UpdateNoSqlAsync(IImmutableList<TodoChangeInfo> todosToUpdate)
        {
            if (_noSqlContext == null || !todosToUpdate.Any()) return false;

            foreach (var todoToUpdate in todosToUpdate)
            {
                switch (todoToUpdate.State)
                {
                    case EntityState.Deleted:
                        {
                            var noSqlTodo = await _noSqlContext.FindAsync<TodoListNoSql>(todoToUpdate.TodoId);
                            _noSqlContext.Remove(noSqlTodo);
                            break;
                        }
                    case EntityState.Modified:
                        {
                            //Note: You need to read the actual Cosmos entity because of the extra columns like id, _rid, etc.
                            //Version 3 might make attach work https://github.com/aspnet/EntityFrameworkCore/issues/13633
                            var noSqlTodo = await _noSqlContext.FindAsync<TodoListNoSql>(todoToUpdate.TodoId);
                            var update = await _sqlContext.Set<Todo>()
                                .ProjectTo<TodoListNoSql>(SqlToNoSqlMapper)
                                .SingleAsync(x => x.TodoId == todoToUpdate.TodoId);
                            SqlToNoSqlMapper.CreateMapper().Map(update, noSqlTodo);
                            break;
                        }
                    case EntityState.Added:
                        var newTodo = await _sqlContext.Set<Todo>()
                            .ProjectTo<TodoListNoSql>(SqlToNoSqlMapper)
                            .SingleAsync(x => x.TodoId == todoToUpdate.TodoId);
                        _noSqlContext.Add(newTodo);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }
    }
}