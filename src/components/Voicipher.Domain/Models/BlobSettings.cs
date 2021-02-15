using System;

namespace Voicipher.Domain.Models
{
    public record GetBlobSettings : BlobSettings
    {
        public GetBlobSettings(string fileName, Guid userId, Guid audioFileId)
            : base(audioFileId, userId)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }

    public record UploadBlobSettings : BlobSettings
    {
        public UploadBlobSettings(string filePath, Guid userId, Guid audioFileId)
            : base(audioFileId, userId)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }

    public record DeleteBlobSettings : BlobSettings
    {
        public DeleteBlobSettings(string fileName, Guid userId, Guid audioFileId)
            : base(audioFileId, userId)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }

    public record BlobSettings : BlobContainerSettings
    {
        public BlobSettings(Guid audioFileId, Guid userId)
            : base(userId)
        {
            AudioFileId = audioFileId.ToString();
        }

        public string AudioFileId { get; }
    }

    public record BlobContainerSettings
    {
        public BlobContainerSettings(Guid userId)
        {
            ContainerName = userId.ToString();
        }

        public string ContainerName { get; }
    }
}
