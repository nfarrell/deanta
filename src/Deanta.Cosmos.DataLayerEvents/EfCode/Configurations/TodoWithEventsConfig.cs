
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deanta.Cosmos.DataLayerEvents.EfCode.Configurations
{
    public class TodoWithEventsConfig : IEntityTypeConfiguration<TodoWithEvents>
    {
        public void Configure (EntityTypeBuilder<TodoWithEvents> entity)
        {
            entity.HasKey(p => p.TodoId);
            
            entity.HasQueryFilter(p => !p.SoftDeleted);

            entity.HasMany(p => p.Comments)  
                .WithOne()                     
                .HasForeignKey(p => p.TodoId);
        }
    }
}