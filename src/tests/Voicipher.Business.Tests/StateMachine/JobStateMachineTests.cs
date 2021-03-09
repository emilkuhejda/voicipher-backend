using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
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
using Voicipher.Domain.Validation;
using Xunit;

namespace Voicipher.Business.Tests.StateMachine
{
    public class JobStateMachineTests
    {
        [Fact]
        public async Task DoInitAsync_InitializesMachineState()
        {
            // Arrange
            const string expectedFileName = "file-name.wav";

            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFile = new AudioFile { Id = Guid.NewGuid() };

            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
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
                audioFileRepositoryMock.Object,
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

            Assert.Equal(audioFile.Id, jobStateMachine.StateMachineContext.AudioFile.Id);
            Assert.Equal(backgroundJob.Id, jobStateMachine.StateMachineContext.BackgroundJob.Id);
        }

        [Fact]
        public async Task DoInitAsync_InitializesFromMachineState_ReturnsJobStateValidating()
        {
            // Arrange
            const string expectedFileName = "file-name.wav";

            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFile = new AudioFile { Id = Guid.NewGuid() };

            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-validating.json"));
            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var jobStateMachine = new JobStateMachine(
                Mock.Of<ICanRunRecognitionCommand>(),
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                Mock.Of<IWavFileService>(),
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                audioFileRepositoryMock.Object,
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
            Assert.Equal(JobState.Validating, jobStateMachine.MachineState.JobState);
            Assert.Equal(1, jobStateMachine.MachineState.Attempt);
            Assert.Equal(expectedFileName, jobStateMachine.MachineState.FileName);
            Assert.Null(jobStateMachine.MachineState.WavSourceFileName);
            Assert.Equal($"{backgroundJob.AudioFileId}.json", jobStateMachine.MachineState.StateFileName);
            Assert.Equal(DateTime.MinValue, jobStateMachine.MachineState.DateCompletedUtc);
            Assert.Empty(jobStateMachine.MachineState.TranscribedAudioFiles);
            Assert.True(jobStateMachine.MachineState.IsRestored);

            Assert.Equal(audioFile.Id, jobStateMachine.StateMachineContext.AudioFile.Id);
            Assert.Equal(backgroundJob.Id, jobStateMachine.StateMachineContext.BackgroundJob.Id);
        }

        [Fact]
        public async Task DoValidationAsync_ValidationSuccess()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var loggerMock = new Mock<ILogger>();

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-initialized.json"));
            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(new AudioFile());
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var jobStateMachine = new JobStateMachine(
                canRunRecognitionCommandMock.Object,
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                Mock.Of<IWavFileService>(),
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await jobStateMachine.DoValidationAsync(default);

            // Assert
            Assert.Equal(JobState.Validated, jobStateMachine.MachineState.JobState);
        }

        [Fact]
        public async Task DoValidationAsync_ThrowsException_ValidationFailure()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var loggerMock = new Mock<ILogger>();

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult(new OperationError(string.Empty)));
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-initialized.json"));
            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(new AudioFile());
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var jobStateMachine = new JobStateMachine(
                canRunRecognitionCommandMock.Object,
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                Mock.Of<IWavFileService>(),
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act & Assert
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await jobStateMachine.DoValidationAsync(default));
        }

        [Fact]
        public async Task DoConvertingAsync_ConvertingSuccess()
        {
            // Arrange
            const string expectedSourceFileName = "file.voc";

            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFile = new AudioFile { Id = new Guid("884b0bf3-ca58-45ae-951b-b1fae46b0395") };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.RunConversionToWavAsync(It.IsAny<AudioFile>(), default))
                .ReturnsAsync(expectedSourceFileName);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-validated.json"));
            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(audioFile);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var jobStateMachine = new JobStateMachine(
                canRunRecognitionCommandMock.Object,
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                wavFileServiceMock.Object,
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await jobStateMachine.DoConvertingAsync(default);

            // Assert
            diskStorageMock.Verify(x => x.DeleteFolder(It.Is<string>(p => p == audioFile.Id.ToString())));
            wavFileServiceMock.Verify(x => x.RunConversionToWavAsync(It.Is<AudioFile>(a => a.Id == audioFile.Id), default));

            Assert.Equal(expectedSourceFileName, audioFile.SourceFileName);
            Assert.Equal(expectedSourceFileName, jobStateMachine.MachineState.WavSourceFileName);
            Assert.Equal(JobState.Converted, jobStateMachine.MachineState.JobState);
        }

        private BackgroundJob CreateBackgroundJob()
        {
            return CreateBackgroundJob(string.Empty);
        }

        private BackgroundJob CreateBackgroundJob(string fileName)
        {
            var parameters = new Dictionary<BackgroundJobParameter, object>
            {
                {BackgroundJobParameter.FileName, fileName}
            };

            return new BackgroundJob
            {
                Id = new Guid("53e772e7-5615-4947-aaf6-272ec06f466a"),
                AudioFileId = new Guid("884b0bf3-ca58-45ae-951b-b1fae46b0395"),
                UserId = new Guid("50c26d76-087e-48c3-9f46-4a9f506845b8"),
                JobState = JobState.Idle,
                DateCreatedUtc = DateTime.UtcNow,
                Parameters = JsonConvert.SerializeObject(parameters)
            };
        }

        private async Task<string> GetJsonAsync(string fileName)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var pathToJson = Path.Combine(directory, "JsonData", fileName);
            return await File.ReadAllTextAsync(pathToJson);
        }
    }
}
