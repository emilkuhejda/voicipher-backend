using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.StateMachine;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Settings;
using Xunit;

namespace Voicipher.Business.Tests.StateMachine
{
    public class JobStateMachineTests
    {
        [Fact]
        public async Task TestStateMachine()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.SplitAudioFileAsync(It.IsAny<AudioFile>(), default))
                .ReturnsAsync(new[] { new TranscribedAudioFile() });
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(new AudioFile());

            var parameters = new Dictionary<BackgroundJobParameter, object>
            {
                {
                    BackgroundJobParameter.AudioFiles,
                    new TranscribedAudioFile[]{new() {Id = Guid.NewGuid()}, new() {Id = Guid.NewGuid()}, new() {Id = Guid.NewGuid()}}
                }
            };

            var backgroundJob = new BackgroundJob
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AudioFileId = Guid.NewGuid(),
                JobState = JobState.Idle,
                Attempt = 0,
                Parameters = JsonConvert.SerializeObject(parameters),
                DateCreatedUtc = DateTime.UtcNow,
                DateCompletedUtc = DateTime.MinValue
            };

            var jobStateMachine = new JobStateMachine(
                canRunRecognitionCommandMock.Object,
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                wavFileServiceMock.Object,
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                Mock.Of<IBlobStorage>(),
                Mock.Of<IAudioFileRepository>(),
                Mock.Of<ITranscribeItemRepository>(),
                unitOfWorkMock.Object,
                Mock.Of<IOptions<AppSettings>>(),
                Mock.Of<ILogger>());

            // Act
            jobStateMachine.DoInit(backgroundJob);
            await jobStateMachine.DoValidationAsync(default);
            await jobStateMachine.DoConvertingAsync(default);
            await jobStateMachine.DoProcessingAsync(default);
            await jobStateMachine.DoCompleteAsync(default);
            await jobStateMachine.SaveAsync(default);

            // Assert
            unitOfWorkMock.Verify(x => x.SaveAsync(default), Times.Once);
            Assert.Equal(JobState.Completed, backgroundJob.JobState);
            Assert.Equal(1, backgroundJob.Attempt);
            Assert.NotEqual(DateTime.MinValue, backgroundJob.DateCompletedUtc);
            Assert.Single(JsonConvert.DeserializeObject<Dictionary<BackgroundJobParameter, TranscribedAudioFile[]>>(backgroundJob.Parameters));
        }
    }
}
