
using Deanta.Cosmos.DataLayerEvents.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deanta.Cosmos.DataLayerEvents.EfCode.Configurations
{
    public class OwnerWithEventsConfig : IEntityTypeConfiguration<OwnerWithEvents>
    {
        public void Configure (EntityTypeBuilder<OwnerWithEvents> entity)
        {
            entity.HasKey(p => p.OwnerId);
            entity.Property(b => b.Name).HasField("_name");
        }
    }
}