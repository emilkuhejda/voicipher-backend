using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.StateMachine;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;
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
                .ReturnsAsync(new[] { new TranscribeAudioFile() });
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(new AudioFile());

            var parameters = new Dictionary<BackgroundJobParameter, object>
            {
                {
                    BackgroundJobParameter.AudioFiles,
                    new TranscribeAudioFile[]{new() {Id = Guid.NewGuid()}, new() {Id = Guid.NewGuid()}, new() {Id = Guid.NewGuid()}}
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
                wavFileServiceMock.Object,
                Mock.Of<ISpeechRecognitionService>(),
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                unitOfWorkMock.Object,
                Mock.Of<IMapper>(),
                Mock.Of<ILogger>());

            // Act
            jobStateMachine.DoInit(backgroundJob);
            await jobStateMachine.DoValidationAsync(default);
            await jobStateMachine.DoConvertingAsync(default);
            await jobStateMachine.DoProcessingAsync(default);
            jobStateMachine.DoCompleteAsync(default);
            await jobStateMachine.SaveAsync(default);

            // Assert
            unitOfWorkMock.Verify(x => x.SaveAsync(default), Times.Once);
            Assert.Equal(JobState.Completed, backgroundJob.JobState);
            Assert.Equal(1, backgroundJob.Attempt);
            Assert.NotEqual(DateTime.MinValue, backgroundJob.DateCompletedUtc);
            Assert.Single(JsonConvert.DeserializeObject<Dictionary<BackgroundJobParameter, TranscribeAudioFile[]>>(backgroundJob.Parameters));
        }
    }
}
