using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class InternalValueConfiguration : IEntityTypeConfiguration<InternalValue>
    {
        public void Configure(EntityTypeBuilder<InternalValue> builder)
        {
            builder.ToTable("InternalValue");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Key).HasMaxLength(100);
            builder.Property(x => x.Value).HasMaxLength(100);
        }
    }
}
