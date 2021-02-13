using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class LanguageVersionConfiguration : IEntityTypeConfiguration<LanguageVersion>
    {
        public void Configure(EntityTypeBuilder<LanguageVersion> builder)
        {
            builder.ToTable("LanguageVersion");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.InformationMessageId).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Message).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Language).IsRequired();
            builder.Property(x => x.SentOnOsx).IsRequired();
            builder.Property(x => x.SentOnAndroid).IsRequired();
        }
    }
}
