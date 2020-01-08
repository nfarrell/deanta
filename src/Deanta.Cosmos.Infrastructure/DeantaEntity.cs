using System;
using System.ComponentModel.DataAnnotations;

namespace Deanta.Cosmos.Infrastructure
{
    public abstract class DeantaEntity<T> where T : struct
    {
        [Key]
        public T Id { get; set; }

        [ConcurrencyCheck]
        public byte[] Version { get; set; } = null!;
    }

    public abstract class DeantaEntity : DeantaEntity<int>
    {

    }
}