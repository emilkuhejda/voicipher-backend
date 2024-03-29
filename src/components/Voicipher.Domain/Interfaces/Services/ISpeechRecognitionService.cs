﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface ISpeechRecognitionService
    {
        bool CanCreateSpeechClientAsync();

        Task<TranscribeItem[]> RecognizeAsync(AudioFile audioFile, TranscribedAudioFile[] transcribedAudioFiles, Guid applicationId, CancellationToken cancellationToken);
    }
}
