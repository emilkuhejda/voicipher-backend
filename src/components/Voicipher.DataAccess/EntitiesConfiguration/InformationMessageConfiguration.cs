using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class InformationMessageConfiguration : IEntityTypeConfiguration<InformationMessage>
    {
        public void Configure(EntityTypeBuilder<InformationMessage> builder)
        {
            builder.ToTable("InformationMessage");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.CampaignName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.DateCreatedUtc).IsRequired();
        }
    }
}
