using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class TranscribeItemConfiguration : IEntityTypeConfiguration<TranscribeItem>
    {
        public void Configure(EntityTypeBuilder<TranscribeItem> builder)
        {
            builder.ToTable("TranscribeItem");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.AudioFileId).IsRequired();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.Alternatives).IsRequired();
            builder.Property(x => x.SourceFileName).HasMaxLength(255);
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime).IsRequired();
            builder.Property(x => x.TotalTime).IsRequired();
            builder.Property(x => x.DateCreatedUtc).IsRequired();
            builder.Property(x => x.DateUpdatedUtc).IsRequired();
            builder.Property(x => x.WasCleaned).IsRequired();
        }
    }
}
