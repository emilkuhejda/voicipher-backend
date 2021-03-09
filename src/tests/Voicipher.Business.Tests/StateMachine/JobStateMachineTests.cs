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
        private const string AudioFileId = "884b0bf3-ca58-45ae-951b-b1fae46b0395";

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

            var audioFile = new AudioFile { Id = new Guid(AudioFileId) };

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

        [Fact]
        public async Task DoSplitAsync_SkipConverting_SourceFileNameInitialized()
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

            var audioFile = new AudioFile { Id = new Guid(AudioFileId) };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.RunConversionToWavAsync(It.IsAny<AudioFile>(), default))
                .ReturnsAsync(expectedSourceFileName);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-converted.json"));
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
            diskStorageMock.Verify(x => x.DeleteFolder(It.IsAny<string>()), Times.Never);
            wavFileServiceMock.Verify(x => x.RunConversionToWavAsync(It.IsAny<AudioFile>(), default), Times.Never);

            Assert.Equal(expectedSourceFileName, audioFile.SourceFileName);
            Assert.Equal(expectedSourceFileName, jobStateMachine.MachineState.WavSourceFileName);
            Assert.Equal(JobState.Converted, jobStateMachine.MachineState.JobState);
        }

        [Fact]
        public async Task DoSplitAsync_SplitsSuccess()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFileId = new Guid(AudioFileId);
            var audioFile = new AudioFile { Id = audioFileId };
            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = Guid.NewGuid(), Path = "path-to-file-1"},
                new() {Id = Guid.NewGuid(), Path = "path-to-file-2"}
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.SplitAudioFileAsync(It.Is<AudioFile>(a => a.Id == audioFileId), default))
                .ReturnsAsync(transcribedAudioFiles);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-converted.json"));
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
                blobStorageMock.Object,
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await jobStateMachine.DoSplitAsync(default);

            // Assert
            Assert.Collection(
                jobStateMachine.MachineState.TranscribedAudioFiles,
                item => Assert.Equal(transcribedAudioFiles[0].Id, item.Id),
                item => Assert.Equal(transcribedAudioFiles[1].Id, item.Id));
            Assert.Equal(JobState.Split, jobStateMachine.MachineState.JobState);

            wavFileServiceMock.Verify(x => x.SplitAudioFileAsync(It.Is<AudioFile>(a => a.Id == audioFileId), default), Times.Once);
            blobStorageMock.Verify(x => x.UploadAsync(It.IsAny<UploadBlobSettings>(), default), Times.Exactly(2));
            diskStorageMock.Verify(x => x.Delete(It.Is<DiskStorageSettings>(d => d == new DiskStorageSettings(audioFile.Id.ToString(), "file.voc"))));
        }

        [Fact]
        public async Task DoSplitAsync_PartialFilesExist_SplitsSuccess()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFileId = new Guid(AudioFileId);
            var audioFile = new AudioFile { Id = audioFileId };
            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = new Guid("14a37bfe-44bb-42ea-9b90-a91f3d5d3adb"), Path = "path-to-file-1"},
                new() {Id = new Guid("9fcd315e-dd0d-46bf-a490-c6e9a8d182e3"), Path = "path-to-file-2"}
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-splitting.json"));
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
                blobStorageMock.Object,
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await jobStateMachine.DoSplitAsync(default);

            // Assert
            Assert.Collection(
                jobStateMachine.MachineState.TranscribedAudioFiles,
                item => Assert.Equal(transcribedAudioFiles[0].Id, item.Id),
                item => Assert.Equal(transcribedAudioFiles[1].Id, item.Id));
            Assert.Equal(JobState.Split, jobStateMachine.MachineState.JobState);

            wavFileServiceMock.Verify(x => x.SplitAudioFileAsync(It.Is<AudioFile>(a => a.Id == audioFileId), default), Times.Never);
            blobStorageMock.Verify(x => x.UploadAsync(It.IsAny<UploadBlobSettings>(), default), Times.Exactly(2));
            diskStorageMock.Verify(x => x.Delete(It.Is<DiskStorageSettings>(d => d == new DiskStorageSettings(audioFile.Id.ToString(), "file.voc"))));
        }

        [Fact]
        public async Task DoSplitAsync_OnlyOnePartialFileExist_SplitsSuccess()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFileId = new Guid(AudioFileId);
            var audioFile = new AudioFile { Id = audioFileId };
            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = new Guid("14a37bfe-44bb-42ea-9b90-a91f3d5d3adb"), Path = "path-to-file"},
                new() {Id = new Guid("9fcd315e-dd0d-46bf-a490-c6e9a8d182e3"), Path = "path-to-file"}
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.SplitAudioFileAsync(It.Is<AudioFile>(a => a.Id == audioFileId), default))
                .ReturnsAsync(transcribedAudioFiles);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.Exists(It.Is<string>(p => p == "path-to-file-2")))
                .Returns(false);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-splitting.json"));
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
                blobStorageMock.Object,
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await jobStateMachine.DoSplitAsync(default);

            // Assert
            Assert.Collection(
                jobStateMachine.MachineState.TranscribedAudioFiles,
                item => Assert.Equal(transcribedAudioFiles[0].Id, item.Id),
                item => Assert.Equal(transcribedAudioFiles[1].Id, item.Id));
            Assert.Equal(JobState.Split, jobStateMachine.MachineState.JobState);

            wavFileServiceMock.Verify(x => x.SplitAudioFileAsync(It.Is<AudioFile>(a => a.Id == audioFileId), default), Times.Once);
            blobStorageMock.Verify(x => x.UploadAsync(It.IsAny<UploadBlobSettings>(), default), Times.Exactly(2));
            diskStorageMock.Verify(x => x.Delete(It.Is<DiskStorageSettings>(d => d == new DiskStorageSettings(audioFile.Id.ToString(), "file.voc"))));
        }

        [Fact]
        public async Task DoSplitAsync_FileNotExists_ThrowsException()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFileId = new Guid(AudioFileId);
            var audioFile = new AudioFile { Id = audioFileId };
            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = new Guid("14a37bfe-44bb-42ea-9b90-a91f3d5d3adb"), Path = "path-to-file-1"},
                new() {Id = new Guid("9fcd315e-dd0d-46bf-a490-c6e9a8d182e3"), Path = "path-to-file-2"}
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.SplitAudioFileAsync(It.Is<AudioFile>(a => a.Id == audioFileId), default))
                .ReturnsAsync(transcribedAudioFiles);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.Exists(It.Is<string>(p => p == transcribedAudioFiles[0].Path)))
                .Returns(false);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-splitting.json"));
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
                blobStorageMock.Object,
                indexMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act & Assert
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await jobStateMachine.DoSplitAsync(default));
        }

        [Fact]
        public async Task DoProcessingAsync_ProcessingSuccess()
        {
            // Arrange
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var speechRecognitionServiceMock = new Mock<ISpeechRecognitionService>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var transcribeItemRepositoryMock = new Mock<ITranscribeItemRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFileId = new Guid(AudioFileId);
            var audioFile = new AudioFile { Id = audioFileId };
            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = new Guid("14a37bfe-44bb-42ea-9b90-a91f3d5d3adb"), Path = "path-to-file",TotalTime = TimeSpan.FromMinutes(1),EndTime = TimeSpan.FromMinutes(1)},
                new() {Id = new Guid("9fcd315e-dd0d-46bf-a490-c6e9a8d182e3"), Path = "path-to-file", TotalTime = TimeSpan.FromMinutes(1),EndTime = TimeSpan.FromMinutes(2)}
            };

            var transcribeItems = new TranscribeItem[]
            {
                new() {Id = Guid.NewGuid()},
                new() {Id = Guid.NewGuid()}
            };

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            speechRecognitionServiceMock
                .Setup(x => x.RecognizeAsync(
                    It.Is<AudioFile>(a => a.Id == audioFileId),
                    It.IsAny<TranscribedAudioFile[]>(),
                    default))
                .ReturnsAsync(transcribeItems);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetJsonAsync("machine-state-split.json"));
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
                Mock.Of<IWavFileService>(),
                speechRecognitionServiceMock.Object,
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                audioFileRepositoryMock.Object,
                transcribeItemRepositoryMock.Object,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act
            await jobStateMachine.DoInitAsync(CreateBackgroundJob(), default);
            await jobStateMachine.DoProcessingAsync(default);

            // Assert
            Assert.Equal(JobState.Processed, jobStateMachine.MachineState.JobState);
            Assert.Equal(transcribedAudioFiles[1].EndTime, jobStateMachine.StateMachineContext.AudioFile.TranscribedTime);

            speechRecognitionServiceMock.Verify(
                x => x.RecognizeAsync(
                    It.Is<AudioFile>(a => a.Id == audioFileId),
                    It.Is<TranscribedAudioFile[]>(t => t.Length == transcribedAudioFiles.Length),
                    default), Times.Once);
            transcribeItemRepositoryMock.Verify(
                x => x.AddRangeAsync(It.Is<TranscribeItem[]>(t => t.Length == transcribeItems.Length), default), Times.Once);
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
                AudioFileId = new Guid(AudioFileId),
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
