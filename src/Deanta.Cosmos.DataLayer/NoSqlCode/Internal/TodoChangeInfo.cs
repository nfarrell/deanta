
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: InternalsVisibleTo("Deanta.Cosmos.Test")]

namespace Deanta.Cosmos.DataLayer.NoSqlCode.Internal
{
    public class TodoChangeInfo 
    {
        /// <summary>
        /// This ctor should be called whenever an entity that has the ITodoId interface
        /// </summary>
        /// <param name="todoId"></param>
        /// <param name="entity"></param>
        private TodoChangeInfo(Guid todoId, EntityEntry entity)
        {
            TodoId = todoId;
            if (entity.Entity is Todo todo) 
            {
                var softDeletedProp = entity.Property(nameof(todo.SoftDeleted));         

                if (softDeletedProp.IsModified)
                {                               
                    State = todo.SoftDeleted   
                        ? EntityState.Deleted   
                        : EntityState.Added;    
                }
                else if (entity.State == EntityState.Deleted)        
                {                               
                    State = todo.SoftDeleted   
                        ? EntityState.Unchanged 
                        : EntityState.Deleted;  
                }
                else
                {
                    State = todo.SoftDeleted 
                        ? EntityState.Unchanged 
                        : entity.State;
                }
            }
            else
            {
                //The entity wasn't a todo, but is related to a todo so we mark the todo as updated
                State = EntityState.Modified; 
            }
        }

        public EntityState State { get; }
        public Guid TodoId { get; }

        /// <summary>
        /// This returns the list of todos that have changes, and how they have changed
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static IImmutableList<TodoChangeInfo> FindTodoChanges(ICollection<EntityEntry> changes, DeantaSqlDbContext context)
        {
            //This finds all the changes using the TodoId
            var todoChanges = changes
                .Select(x => new {entity = x, todoRef = x.Entity as ITodoId})
                .Where(x => x.entity.State != EntityState.Unchanged && x.entity.State != EntityState.Detached && x.todoRef != null)
                .Select(x => new TodoChangeInfo(x.todoRef.TodoId, x.entity)).ToList();
            //Now add any author name changes
            todoChanges.AddRange(AddTodosWhereOwnerHasChanged(changes, context));

            //This de -duplicates the to-do changes, with the To-do entity State taking preference
            var todosDict = new Dictionary<Guid, TodoChangeInfo>();
            foreach (var todoChange in todoChanges)
            {
                if (todosDict.ContainsKey(todoChange.TodoId) && todosDict[todoChange.TodoId].State != EntityState.Modified)
                    continue;

                todosDict[todoChange.TodoId] = todoChange;
            }

            return todosDict.Values.Where(x => x.State != EntityState.Unchanged).ToImmutableList();
        }

        public static List<TodoChangeInfo> AddTodosWhereOwnerHasChanged(ICollection<EntityEntry> changes,
            DeantaSqlDbContext context)
        {
            var ownerChanges = changes
                .Select(x => new { entity = x, authorRef = x.Entity as IOwnerId })
                .Where(x => x.entity.State != EntityState.Unchanged && x.entity.State != EntityState.Detached && x.authorRef != null);
            var result = new List<TodoChangeInfo>();
            foreach (var authorChange in ownerChanges)
            {
                result.AddRange(context.TodoOwners
                    .Where(x => x.OwnerId == authorChange.authorRef.OwnerId)
                    .Select(x => new TodoChangeInfo(x.TodoId, authorChange.entity)));
            }

            return result;
        }
    }
}