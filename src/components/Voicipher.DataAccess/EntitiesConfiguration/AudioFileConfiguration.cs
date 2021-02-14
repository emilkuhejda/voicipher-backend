using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class AudioFileConfiguration : IEntityTypeConfiguration<AudioFile>
    {
        public void Configure(EntityTypeBuilder<AudioFile> builder)
        {
            builder.ToTable("AudioFile");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Language).IsRequired().HasMaxLength(20);
            builder.Property(x => x.RecognitionState).IsRequired();
            builder.Property(x => x.OriginalSourceFileName).HasMaxLength(100);
            builder.Property(x => x.SourceFileName).HasMaxLength(100);
            builder.Property(x => x.Storage).IsRequired();
            builder.Property(x => x.UploadStatus).IsRequired();
            builder.Property(x => x.TotalTime).IsRequired();
            builder.Property(x => x.TranscribedTime).IsRequired();
            builder.Property(x => x.DateCreated).IsRequired();
            builder.Property(x => x.DateUpdatedUtc).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.IsPermanentlyDeleted).IsRequired();
            builder.Property(x => x.WasCleaned).IsRequired();

            builder.HasMany(x => x.TranscribeItems).WithOne(x => x.AudioFile).HasForeignKey(x => x.AudioFileId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
