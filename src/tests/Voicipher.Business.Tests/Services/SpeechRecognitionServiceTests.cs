using System;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;
using Xunit;

namespace Voicipher.Business.Tests.Services
{
    public class SpeechRecognitionServiceTests
    {
        [Fact]
        public async Task RecognizeAsync_RecognitionSuccess()
        {
            // Arrange
            var speechClientFactoryMock = new Mock<ISpeechClientFactory>();
            var speechClientMock = new Mock<SpeechClient>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var optionMock = new Mock<IOptions<AppSettings>>();
            var loggerMock = new Mock<ILogger>();

            speechClientFactoryMock.Setup(x => x.CreateClient()).Returns(speechClientMock.Object);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            optionMock.Setup(x => x.Value).Returns(new AppSettings());
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var speechRecognitionService = new SpeechRecognitionService(
                speechClientFactoryMock.Object,
                Mock.Of<IAudioFileProcessingChannel>(),
                Mock.Of<IMessageCenterService>(),
                Mock.Of<IFileAccessService>(),
                indexMock.Object,
                optionMock.Object,
                loggerMock.Object);

            var (audioFile, transcribedAudioFiles) = GenerateMockData();

            // Act
            await speechRecognitionService.RecognizeAsync(audioFile, transcribedAudioFiles, default);

            // Assert
        }

        private (AudioFile audioFile, TranscribedAudioFile[] transcribedAudioFiles) GenerateMockData()
        {
            var audioFile = new AudioFile { Id = Guid.NewGuid() };
            var transcribedAudioFiles = new TranscribedAudioFile[]
            {
                new() {Id = new Guid(), AudioFileId = audioFile.Id},
                //new() {Id = new Guid(), AudioFileId = audioFile.Id},
            };

            return (audioFile, transcribedAudioFiles);
        }
    }
}
