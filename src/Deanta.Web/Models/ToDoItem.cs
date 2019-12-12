using System;

namespace Deanta.Web
{
    public class ToDoItem : DeantaEntity<Guid> //Guid for faster inserts...
    {
        public string Description { get; set; }

        public DeantaUser User { get; set; }
    }
}
