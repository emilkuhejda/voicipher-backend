using Voicipher.Domain.Enums;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Domain.Configuration
{
    public static class InternalValues
    {
        public static GetInternalValuePayload<int> AudioFilesCleanUpInDays { get; } = new(InternalValueKey.AudioFilesCleanUpInDays, 30);

        public static GetInternalValuePayload<int> TranscribeItemsCleanUpInDays { get; } = new GetInternalValuePayload<int>(InternalValueKey.TranscribeItemsCleanUpInDays, 60);
    }
}
