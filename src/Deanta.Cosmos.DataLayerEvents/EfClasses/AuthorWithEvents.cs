
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Deanta.Cosmos.DataLayerEvents.DomainEvents;
using GenericEventRunner.ForEntities;

namespace Deanta.Cosmos.DataLayerEvents.EfClasses
{
    public class OwnerWithEvents : EntityEvents
    {
        public const int NameLength = 100;
        public const int EmailLength = 100;

        private string _name;
        private HashSet<TodoOwnerWithEvents> _todosLink;

        private OwnerWithEvents() { }

        public OwnerWithEvents(string name, string email)
        {
            _name = name;
            Email = email;
        }

        public Guid OwnerId { get;  private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(NameLength)]
        public string Name
        {
            get => _name;
            private set
            {
                if (value != _name)
                    AddEvent(new OwnerNameUpdatedEvent(this));
                _name = value;
            }
        }

        [MaxLength(EmailLength)]
        public string Email { get; }

        //------------------------------
        //Relationships

        public ICollection<TodoOwnerWithEvents> TodosLink => _todosLink?.ToList();

        public void ChangeName(string newOwnerName)
        {
            Name = newOwnerName;
        }
    }

}