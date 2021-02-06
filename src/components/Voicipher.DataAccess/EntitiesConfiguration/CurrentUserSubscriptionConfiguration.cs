using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class CurrentUserSubscriptionConfiguration : IEntityTypeConfiguration<CurrentUserSubscription>
    {
        public void Configure(EntityTypeBuilder<CurrentUserSubscription> builder)
        {
            builder.ToTable("CurrentUserSubscription");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Ticks).IsRequired();
            builder.Property(x => x.DateUpdatedUtc).IsRequired();
        }
    }
}
