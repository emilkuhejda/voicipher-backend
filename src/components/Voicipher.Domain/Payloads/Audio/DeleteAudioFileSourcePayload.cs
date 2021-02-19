using System;

namespace Voicipher.Domain.Payloads.Audio
{
    public record DeleteAudioFileSourcePayload
    {
        public DeleteAudioFileSourcePayload(Guid audioFileId, Guid userId)
        {
            AudioFileId = audioFileId;
            UserId = userId;
        }

        public Guid AudioFileId { get; }

        public Guid UserId { get; }
    }
}
