using System;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Infrastructure
{
    public abstract class DeantaEntity<T> where T : struct
    {
        [Key]
        public T Id { get; set; }

        [ConcurrencyCheck]
        public Byte[] Version { get; set; } = null!;
    }

    public abstract class DeantaEntity : DeantaEntity<int>
    {

    }

    public class ToDoItem : DeantaEntity<Guid> //Guid for faster inserts...
    {
        public string Description { get; set; }

        public ApplicationUser User { get; set; }
    }
}
