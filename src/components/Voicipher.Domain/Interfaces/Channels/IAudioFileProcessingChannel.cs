﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Channels
{
    public interface IAudioFileProcessingChannel
    {
        IAsyncEnumerable<RecognitionFile> ReadAllAsync(CancellationToken cancellationToken = default);

        Task<bool> AddFileAsync(RecognitionFile recognitionFile, CancellationToken cancellationToken = default);

        bool IsProcessingForUser(Guid userId);

        void FinishProcessing(RecognitionFile recognitionFile);
    }
}
