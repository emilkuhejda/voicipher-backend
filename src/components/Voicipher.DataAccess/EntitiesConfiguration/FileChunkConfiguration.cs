using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class FileChunkConfiguration : IEntityTypeConfiguration<FileChunk>
    {
        public void Configure(EntityTypeBuilder<FileChunk> builder)
        {
            builder.ToTable("FileChunk");
        }
    }
}
