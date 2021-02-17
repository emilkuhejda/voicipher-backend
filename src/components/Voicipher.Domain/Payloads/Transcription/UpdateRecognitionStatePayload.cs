using System;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads.Transcription
{
    public record UpdateRecognitionStatePayload
    {
        public UpdateRecognitionStatePayload(Guid audioFileId, Guid userId, Guid applicationId, RecognitionState recognitionState)
        {
            AudioFileId = audioFileId;
            UserId = userId;
            ApplicationId = applicationId;
            RecognitionState = recognitionState;
        }

        public Guid AudioFileId { get; }

        public Guid UserId { get; }

        public Guid ApplicationId { get; }

        public RecognitionState RecognitionState { get; }
    }
}
