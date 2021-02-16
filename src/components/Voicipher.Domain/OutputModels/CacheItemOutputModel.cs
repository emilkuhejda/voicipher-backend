using System;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.OutputModels
{
    public record EmptyCacheItemOutputModel : CacheItemOutputModel
    {
        public EmptyCacheItemOutputModel()
            : base(Guid.Empty, RecognitionState.None, 0)
        {
        }
    }

    public record CacheItemOutputModel
    {
        public CacheItemOutputModel(Guid fileItemId, RecognitionState recognitionState, double percentageDone)
        {
            FileItemId = fileItemId;
            RecognitionState = recognitionState;
            PercentageDone = percentageDone;
        }

        public Guid FileItemId { get; }

        public RecognitionState RecognitionState { get; }

        public double PercentageDone { get; }
    }
}
