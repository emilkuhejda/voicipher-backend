using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.StateMachine;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;
using Xunit;

namespace Voicipher.Business.Tests.StateMachine
{
    public class JobStateMachineTests
    {
        [Fact]
        public async Task DoInitAsync_InitializesMachineState()
        {
            // Arrange
            const string expectedFileName = "File name";

            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var loggerMock = new Mock<ILogger>();

            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var jobStateMachine = new JobStateMachine(
                Mock.Of<ICanRunRecognitionCommand>(),
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                Mock.Of<IWavFileService>(),
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                Mock.Of<IFileAccessService>(),
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                Mock.Of<IAudioFileRepository>(),
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            var backgroundJob = CreateBackgroundJob(expectedFileName);

            // Act
            await jobStateMachine.DoInitAsync(backgroundJob, default);

            // Assert
            Assert.Equal(backgroundJob.Id, jobStateMachine.MachineState.JobId);
            Assert.Equal(backgroundJob.UserId, jobStateMachine.MachineState.UserId);
            Assert.Equal(backgroundJob.AudioFileId, jobStateMachine.MachineState.AudioFileId);
            Assert.Equal(JobState.Initialized, jobStateMachine.MachineState.JobState);
            Assert.Equal(1, jobStateMachine.MachineState.Attempt);
            Assert.Equal(expectedFileName, jobStateMachine.MachineState.FileName);
            Assert.Null(jobStateMachine.MachineState.WavSourceFileName);
            Assert.Equal($"{backgroundJob.AudioFileId}.json", jobStateMachine.MachineState.StateFileName);
            Assert.Equal(DateTime.MinValue, jobStateMachine.MachineState.DateCompletedUtc);
            Assert.Empty(jobStateMachine.MachineState.TranscribedAudioFiles);
            Assert.False(jobStateMachine.MachineState.IsRestored);
        }

        private BackgroundJob CreateBackgroundJob(string fileName)
        {
            var parameters = new Dictionary<BackgroundJobParameter, object>
            {
                {BackgroundJobParameter.FileName, fileName}
            };

            return new BackgroundJob
            {
                Id = Guid.NewGuid(),
                AudioFileId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                JobState = JobState.Idle,
                DateCreatedUtc = DateTime.UtcNow,
                Parameters = JsonConvert.SerializeObject(parameters)
            };
        }
    }
}
