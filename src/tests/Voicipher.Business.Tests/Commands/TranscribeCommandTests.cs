using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using Serilog;
using Voicipher.Business.Commands.Audio;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Payloads.Audio;
using Xunit;

namespace Voicipher.Business.Tests.Commands
{
    public class TranscribeCommandTests
    {
        [Fact]
        public async Task ExecuteCommand_ReturnsSuccess()
        {
            // Arrange
            const string expectedLanguage = "en-US";

            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var audioFileProcessingChannelMock = new Mock<IAudioFileProcessingChannel>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var backgroundJobRepositoryMock = new Mock<IBackgroundJobRepository>();
            var loggerMock = new Mock<ILogger>();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UploadStatus = UploadStatus.Completed,
                RecognitionState = RecognitionState.None,
                TotalTime = TimeSpan.FromMinutes(10)
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), It.Is<ClaimsPrincipal>(c => c == claimsPrincipal), default))
                .ReturnsAsync(new CommandResult());
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var transcribeCommand = new TranscribeCommand(
                canRunRecognitionCommandMock.Object,
                audioFileProcessingChannelMock.Object,
                audioFileRepositoryMock.Object,
                backgroundJobRepositoryMock.Object,
                loggerMock.Object);

            var transcribePayload = new TranscribePayload(audioFile.Id, expectedLanguage, 0, 0, Guid.NewGuid());

            // Act
            var commandResult = await transcribeCommand.ExecuteAsync(transcribePayload, claimsPrincipal, default);

            // Assert
            Assert.Equal(expectedLanguage, audioFile.Language);
            Assert.Equal(TimeSpan.Zero, audioFile.TranscriptionStartTime);
            Assert.Equal(TimeSpan.FromMinutes(10), audioFile.TranscriptionEndTime);
            Assert.True(commandResult.IsSuccess);

            audioFileProcessingChannelMock
                .Verify(x => x.AddFileAsync(It.Is<RecognitionFile>(r => r.AudioFileId == audioFile.Id), default));
        }

        [Fact]
        public async Task ExecuteCommand_TranscribePart_ReturnsSuccess()
        {
            // Arrange
            const string expectedLanguage = "en-US";

            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var audioFileProcessingChannelMock = new Mock<IAudioFileProcessingChannel>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var backgroundJobRepositoryMock = new Mock<IBackgroundJobRepository>();
            var loggerMock = new Mock<ILogger>();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UploadStatus = UploadStatus.Completed,
                RecognitionState = RecognitionState.None,
                TotalTime = TimeSpan.FromMinutes(10)
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), It.Is<ClaimsPrincipal>(c => c == claimsPrincipal), default))
                .ReturnsAsync(new CommandResult());
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var transcribeCommand = new TranscribeCommand(
                canRunRecognitionCommandMock.Object,
                audioFileProcessingChannelMock.Object,
                audioFileRepositoryMock.Object,
                backgroundJobRepositoryMock.Object,
                loggerMock.Object);

            var transcribePayload = new TranscribePayload(
                audioFile.Id, expectedLanguage,
                (uint)TimeSpan.FromMinutes(2).TotalSeconds,
                (uint)TimeSpan.FromMinutes(8).TotalSeconds,
                Guid.NewGuid());

            // Act
            var commandResult = await transcribeCommand.ExecuteAsync(transcribePayload, claimsPrincipal, default);

            // Assert
            Assert.Equal(expectedLanguage, audioFile.Language);
            Assert.Equal(TimeSpan.FromMinutes(2), audioFile.TranscriptionStartTime);
            Assert.Equal(TimeSpan.FromMinutes(8), audioFile.TranscriptionEndTime);
            Assert.True(commandResult.IsSuccess);

            audioFileProcessingChannelMock
                .Verify(x => x.AddFileAsync(It.Is<RecognitionFile>(r => r.AudioFileId == audioFile.Id), default));
        }

        [Fact]
        public async Task ExecuteCommand_UnsupportedLanguage_ThrowsException()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var audioFileProcessingChannelMock = new Mock<IAudioFileProcessingChannel>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var backgroundJobRepositoryMock = new Mock<IBackgroundJobRepository>();
            var loggerMock = new Mock<ILogger>();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UploadStatus = UploadStatus.Completed,
                RecognitionState = RecognitionState.None,
                TotalTime = TimeSpan.FromMinutes(10)
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), It.Is<ClaimsPrincipal>(c => c == claimsPrincipal), default))
                .ReturnsAsync(new CommandResult());
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var transcribeCommand = new TranscribeCommand(
                canRunRecognitionCommandMock.Object,
                audioFileProcessingChannelMock.Object,
                audioFileRepositoryMock.Object,
                backgroundJobRepositoryMock.Object,
                loggerMock.Object);

            var transcribePayload = new TranscribePayload(audioFile.Id, "lang", 0, 0, Guid.NewGuid());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OperationErrorException>(async () => await transcribeCommand.ExecuteAsync(transcribePayload, claimsPrincipal, default));

            // Assert
            Assert.Equal(ErrorCode.EC200, exception.ErrorCode);
        }

        [Fact]
        public async Task ExecuteCommand_InvalidEndTime_ThrowsException()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var audioFileProcessingChannelMock = new Mock<IAudioFileProcessingChannel>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var backgroundJobRepositoryMock = new Mock<IBackgroundJobRepository>();
            var loggerMock = new Mock<ILogger>();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UploadStatus = UploadStatus.Completed,
                RecognitionState = RecognitionState.None,
                TotalTime = TimeSpan.FromMinutes(10)
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), It.Is<ClaimsPrincipal>(c => c == claimsPrincipal), default))
                .ReturnsAsync(new CommandResult());
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var transcribeCommand = new TranscribeCommand(
                canRunRecognitionCommandMock.Object,
                audioFileProcessingChannelMock.Object,
                audioFileRepositoryMock.Object,
                backgroundJobRepositoryMock.Object,
                loggerMock.Object);

            var transcribePayload = new TranscribePayload(audioFile.Id, "en-US", 10, 0, Guid.NewGuid());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OperationErrorException>(async () => await transcribeCommand.ExecuteAsync(transcribePayload, claimsPrincipal, default));

            // Assert
            Assert.Equal(ErrorCode.EC600, exception.ErrorCode);
        }
    }
}
