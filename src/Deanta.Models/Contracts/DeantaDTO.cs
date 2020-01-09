using System;

namespace Deanta.Models.Contracts
{
    public class TodoDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsCompleted { get; set; }
    }
}
