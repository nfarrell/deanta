
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deanta.Cosmos.DataLayer.EfCode.Configurations
{
    public class TodoConfig : IEntityTypeConfiguration<Todo>
    {
        public void Configure
            (EntityTypeBuilder<Todo> entity)
        {
            entity.HasQueryFilter(p => !p.SoftDeleted);

            entity.HasMany(p => p.Comments)  
                .WithOne()                     
                .HasForeignKey(p => p.TodoId);

            entity.Metadata
                .FindNavigation(nameof(Todo.Comments))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            entity.Metadata
                .FindNavigation(nameof(Todo.OwnersLink))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}