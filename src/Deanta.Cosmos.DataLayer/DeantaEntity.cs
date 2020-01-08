using System;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Cosmos.DataLayer
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
}