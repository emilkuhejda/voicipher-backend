using System;

namespace Voicipher.Domain.Models
{
    public record GetBlobSettings : BlobSettings
    {
        public GetBlobSettings(string fileName, Guid userId, Guid audioFileId)
            : base(userId, audioFileId)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }

    public record UploadBlobSettings : BlobSettings
    {
        public UploadBlobSettings(string filePath, Guid userId, Guid audioFileId)
            : base(userId, audioFileId)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }

    public record DeleteBlobSettings : BlobSettings
    {
        public DeleteBlobSettings(string fileName, Guid userId, Guid audioFileId)
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
