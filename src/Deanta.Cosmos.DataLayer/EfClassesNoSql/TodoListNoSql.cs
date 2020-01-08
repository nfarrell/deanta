using System;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Cosmos.DataLayer.EfClassesNoSql
{
    public class TodoListNoSql
    {
        [Key]
        public Guid TodoId { get; set; }

        public string Title { get; set; }

        public bool? IsCompleted { get; set; }

        public string Owner { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}