using System;

namespace Deanta.Models.Models
{
    public class User
    {
        public User()
        {
        }

        public User(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
