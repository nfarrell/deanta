
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deanta.Cosmos.DataLayerEvents.EfCode.Configurations
{
    public class TodoOwnerWithEventsConfig : IEntityTypeConfiguration<TodoOwnerWithEvents>
    {
        public void Configure(EntityTypeBuilder<TodoOwnerWithEvents> entity)
        {
            entity.HasKey(p => 
                new { p.TodoId, p.OwnerId }); 

            //-----------------------------
            //Relationships

            entity.HasOne(pt => pt.Todo)        
                .WithMany(p => p.OwnersLink)   
                .HasForeignKey(pt => pt.TodoId);

            entity.HasOne(pt => pt.Owner)        
                .WithMany(t => t.TodosLink)       
                .HasForeignKey(pt => pt.OwnerId);
        }
    }
}