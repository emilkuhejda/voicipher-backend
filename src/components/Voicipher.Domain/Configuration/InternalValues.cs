using Voicipher.Domain.Enums;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValuePayload<int> AudioFilesCleanUpInDays { get; } = new(InternalValueKey.AudioFilesCleanUpInDays, 30);

        public static InternalValuePayload<int> TranscribeItemsCleanUpInDays { get; } = new InternalValuePayload<int>(InternalValueKey.TranscribeItemsCleanUpInDays, 60);
    }
}
