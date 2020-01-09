using System;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Models.Models
{
    /// <summary>
    /// Renamed from Todo to TodoModel because the syntax highlighting was annoying.
    /// </summary>
    public class TodoModel : AuditableEntity
    {
        public TodoModel()
        {
        }

        public TodoModel(Guid todoId, string title)
        {
            TodoId = todoId;
            Title = title;
        }

        [Key]
        public Guid TodoId { get; set; }

        public string Title { get; set; }

        public bool IsCompleted { get; set; }
    }
}
