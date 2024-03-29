﻿using System;
using System.Collections.Generic;

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
        public UploadBlobSettings(string filePath, Guid userId, Guid audioFileId, string contentType)
            : this(filePath, userId, audioFileId, string.Empty, contentType, new Dictionary<string, string>())
        {
        }

        public UploadBlobSettings(string filePath, Guid userId, Guid audioFileId, string fileName, string contentType)
            : this(filePath, userId, audioFileId, fileName, contentType, new Dictionary<string, string>())
        {
        }

        public UploadBlobSettings(string filePath, Guid userId, Guid audioFileId, string fileName, string contentType, Dictionary<string, string> metadata)
            : base(audioFileId, userId)
        {
            FilePath = filePath;
            FileName = fileName;
            ContentType = contentType;
            Metadata = metadata;
        }

        public string FilePath { get; }

        public string FileName { get; }

        public string ContentType { get; }

        public Dictionary<string, string> Metadata { get; }
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
