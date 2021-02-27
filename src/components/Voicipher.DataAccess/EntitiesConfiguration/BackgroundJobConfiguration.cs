using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class BackgroundJobConfiguration : IEntityTypeConfiguration<BackgroundJob>
    {
        public void Configure(EntityTypeBuilder<BackgroundJob> builder)
        {
            builder.ToTable("BackgroundJob");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.AudioFileId).IsRequired();
            builder.Property(x => x.JobState).IsRequired();
            builder.Property(x => x.Attempt).IsRequired();
            builder.Property(x => x.Parameters).IsRequired();
            builder.Property(x => x.Exception);
            builder.Property(x => x.DateCreatedUtc).IsRequired();
            builder.Property(x => x.DateCompletedUtc).IsRequired();
        }
    }
}
