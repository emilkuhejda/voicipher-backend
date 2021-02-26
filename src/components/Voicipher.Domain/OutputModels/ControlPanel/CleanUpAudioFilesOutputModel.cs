using System;
using System.Collections.Generic;
using System.Linq;

namespace Voicipher.Domain.OutputModels.ControlPanel
{
    public record CleanUpAudioFilesOutputModel
    {
        public CleanUpAudioFilesOutputModel()
            : this(0, new Dictionary<Guid, Guid[]>(), new Dictionary<Guid, Guid[]>())
        {
        }

        public CleanUpAudioFilesOutputModel(int audioFilesToBackup, Dictionary<Guid, Guid[]> succeededIds, Dictionary<Guid, Guid[]> failedIds)
        {
            AudioFilesToBackup = audioFilesToBackup;
            SucceededTotal = succeededIds.Sum(x => x.Value.Length);
            FailedTotal = failedIds.Sum(x => x.Value.Length);
            SucceededIds = succeededIds;
            FailedIds = failedIds;
        }

        public int AudioFilesToBackup { get; }

        public int SucceededTotal { get; }

        public int FailedTotal { get; }

        public Dictionary<Guid, Guid[]> SucceededIds { get; }

        public Dictionary<Guid, Guid[]> FailedIds { get; }
    }
}
