using System;

namespace Voicipher.Domain.Models
{
    public record UploadBlobSettings : BlobSettings
    {
        public UploadBlobSettings(Guid userId, Guid audioFileId, string filePath)
            : base(userId, audioFileId)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }

    public record DeleteBlobSettings : BlobSettings
    {
        public DeleteBlobSettings(Guid userId, Guid audioFileId, string fileName)
            : base(userId, audioFileId)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }

    public record BlobSettings
    {
        public BlobSettings(Guid userId, Guid audioFileId)
        {
            ContainerName = userId.ToString();
            AudioFileId = audioFileId.ToString();
        }

        public string AudioFileId { get; }

        public string ContainerName { get; }
    }
}
