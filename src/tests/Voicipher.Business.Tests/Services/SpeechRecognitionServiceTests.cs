using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Google.Cloud.Speech.V1;
using Moq;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Xunit;

namespace Voicipher.Business.Tests.Services
{
    public class SpeechRecognitionServiceTests
    {
        [Fact]
        public async Task RecognizeAsync_RecognitionSuccess()
        {
            // Arrange
            var speechClientMock = new Mock<SpeechClient>();
            var speechClientFactoryMock = new Mock<ISpeechClientFactory>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var loggerMock = new Mock<ILogger>();

            speechClientFactoryMock
                .Setup(x => x.CreateClient())
                .Returns(speechClientMock.Object);
            diskStorageMock.Setup(x => x.GetDirectoryPath(It.IsAny<string>())).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var speechRecognitionService = new FakeSpeechRecognitionService(
                speechClientFactoryMock.Object,
                Mock.Of<IAudioFileProcessingChannel>(),
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                indexMock.Object,
                loggerMock.Object);

            var (audioFile, transcribedAudioFiles) = GenerateMockData();

            // Act
            var transcribeItems = await speechRecognitionService.RecognizeAsync(audioFile, transcribedAudioFiles, Guid.NewGuid(), default);

            // Assert
            Assert.Collection(transcribeItems,
                item =>
                {
                    Assert.Equal(audioFile.Id, item.AudioFileId);
                    Assert.Equal(transcribedAudioFiles[0].Id, item.Id);
                    Assert.True(!string.IsNullOrWhiteSpace(item.Alternatives));
                },
                item =>
                {
                    Assert.Equal(audioFile.Id, item.AudioFileId);
                    Assert.Equal(transcribedAudioFiles[1].Id, item.Id);
                    Assert.True(!string.IsNullOrWhiteSpace(item.Alternatives));
                });

            fileAccessServiceMock.Verify(x => x.ReadAllTextAsync(It.IsAny<string>(), default), Times.Never);
            diskStorageMock.Verify(x => x.UploadAsync(It.IsAny<byte[]>(), It.IsAny<DiskStorageSettings>(), default), Times.Exactly(2));
        }

        [Fact]
        public async Task RecognizeAsync_ReadsDataFromJsonFile_RecognitionSuccess()
        {
            // Arrange
            var speechClientMock = new Mock<SpeechClient>();
            var speechClientFactoryMock = new Mock<ISpeechClientFactory>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var loggerMock = new Mock<ILogger>();

            speechClientFactoryMock
                .Setup(x => x.CreateClient())
                .Returns(speechClientMock.Object);
            diskStorageMock.Setup(x => x.GetDirectoryPath(It.IsAny<string>())).Returns(string.Empty);
            fileAccessServiceMock
                .Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);
            fileAccessServiceMock
                .Setup(x => x.ReadAllTextAsync(It.IsAny<string>(), default))
                .ReturnsAsync(await GetSerializedRecognizedResult());
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var speechRecognitionService = new SpeechRecognitionService(
                speechClientFactoryMock.Object,
                Mock.Of<IAudioFileProcessingChannel>(),
                Mock.Of<IMessageCenterService>(),
                fileAccessServiceMock.Object,
                indexMock.Object,
                loggerMock.Object);

            var (audioFile, transcribedAudioFiles) = GenerateMockData();

            // Act
            var transcribeItems = await speechRecognitionService.RecognizeAsync(audioFile, transcribedAudioFiles, Guid.NewGuid(), default);

            // Assert
            Assert.Collection(transcribeItems,
                item =>
                {
                    Assert.Equal(audioFile.Id, item.AudioFileId);
                    Assert.Equal(transcribedAudioFiles[0].Id, item.Id);
                    Assert.True(!string.IsNullOrWhiteSpace(item.Alternatives));
                },
                item =>
                {
                    Assert.Equal(audioFile.Id, item.AudioFileId);
                    Assert.Equal(transcribedAudioFiles[1].Id, item.Id);
                    Assert.True(!string.IsNullOrWhiteSpace(item.Alternatives));
                });

            fileAccessServiceMock.Verify(x => x.ReadAllTextAsync(It.IsAny<string>(), default), Times.Exactly(2));
            diskStorageMock.Verify(x => x.UploadAsync(It.IsAny<byte[]>(), It.IsAny<DiskStorageSettings>(), default), Times.Never);
        }

        private (AudioFile audioFile, TranscribedAudioFile[] transcribedAudioFiles) GenerateMockData()
        {
            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Language = "en-US"
            };

            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = Guid.NewGuid(), AudioFileId = audioFile.Id},
                new() {Id = Guid.NewGuid(), AudioFileId = audioFile.Id}
            };

            return (audioFile, transcribedAudioFiles);
        }

        private Task<string> GetSerializedRecognizedResult(bool isIncomplete = false)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var fileType = isIncomplete ? "incomplete" : "complete";
            var pathToJson = Path.Combine(directory, "JsonData", $"recognized-result-{fileType}.json");
            return File.ReadAllTextAsync(pathToJson);
        }
    }
}
