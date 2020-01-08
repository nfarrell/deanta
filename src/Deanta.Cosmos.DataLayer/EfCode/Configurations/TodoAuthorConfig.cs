
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deanta.Cosmos.DataLayer.EfCode.Configurations
{
    public class TodoOwnerConfig : IEntityTypeConfiguration<TodoOwner>
    {
        public void Configure(EntityTypeBuilder<TodoOwner> entity)
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