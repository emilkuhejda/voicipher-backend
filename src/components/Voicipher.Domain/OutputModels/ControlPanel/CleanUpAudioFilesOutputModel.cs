using System;
using System.Collections.Generic;
using System.Linq;

namespace Voicipher.Domain.OutputModels.ControlPanel
{
    public record CleanUpAudioFilesOutputModel
    {
        public CleanUpAudioFilesOutputModel()
            : this(0, new Dictionary<Guid, IList<Guid>>(), new Dictionary<Guid, IList<Guid>>())
        {
        }

        public CleanUpAudioFilesOutputModel(int audioFilesToBackup, Dictionary<Guid, IList<Guid>> succeededIds, Dictionary<Guid, IList<Guid>> failedIds)
        {
            AudioFilesToBackup = audioFilesToBackup;
            SucceededTotal = succeededIds.Sum(x => x.Value.Count);
            FailedTotal = failedIds.Sum(x => x.Value.Count);
            SucceededIds = succeededIds;
            FailedIds = failedIds;
        }

        public int AudioFilesToBackup { get; }

        public int SucceededTotal { get; }

        public int FailedTotal { get; }

        public Dictionary<Guid, IList<Guid>> SucceededIds { get; }

        public Dictionary<Guid, IList<Guid>> FailedIds { get; }
    }
}
