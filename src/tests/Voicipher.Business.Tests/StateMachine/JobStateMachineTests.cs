using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Voicipher.Business.StateMachine;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
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
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .Returns(Task.FromResult(new AudioFile()));
            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .Returns(Task.FromResult(new CommandResult()));

            var backgroundJob = new BackgroundJob
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AudioFileId = Guid.NewGuid(),
                JobState = JobState.Idle,
                Attempt = 1,
                Parameters = JsonConvert.SerializeObject(new Dictionary<BackgroundJobParameter, object>()),
                DateCreatedUtc = DateTime.UtcNow
            };

            var jobStateMachine = new JobStateMachine(audioFileRepositoryMock.Object, canRunRecognitionCommandMock.Object, unitOfWorkMock.Object);

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
        }
    }
}
