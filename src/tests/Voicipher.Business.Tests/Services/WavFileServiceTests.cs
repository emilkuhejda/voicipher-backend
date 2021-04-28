using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using Moq;
using NAudio.Wave;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.Business.Tests.Stubs;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Xunit;

namespace Voicipher.Business.Tests.Services
{
    public class WavFileServiceTests
    {
        private const float ExtraSeconds = 0.25f;

        [Fact]
        public async Task SplitAudioFileAsync_ReturnsAudioFiles()
        {
            // Arrange
            var subscriptionTime = TimeSpan.FromMinutes(5);

            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var currentUserSubscriptionRepositoryMock = new Mock<ICurrentUserSubscriptionRepository>();
            var loggerMock = new Mock<ILogger>();

            var sampleBytes = await GetSampleBytes();

            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            diskStorageMock.Setup(x => x.GetDirectoryPath(It.IsAny<string>())).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock.Setup(x => x.ReadAllBytesAsync(It.IsAny<string>(), default)).ReturnsAsync(sampleBytes);
            fileAccessServiceMock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());
            currentUserSubscriptionRepositoryMock.Setup(x => x.GetRemainingTimeAsync(It.IsAny<Guid>(), default)).ReturnsAsync(subscriptionTime);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var wavFileService = new WavFileService(
                fileAccessServiceMock.Object,
                blobStorageMock.Object,
                indexMock.Object,
                currentUserSubscriptionRepositoryMock.Object,
                loggerMock.Object);

            // Act
            var transcribedAudioFiles = await wavFileService.SplitAudioFileAsync(new AudioFile(), default);

            // Assert
            var totalTime = TimeSpan.FromTicks(transcribedAudioFiles.Select(x => x.TotalTime).Sum(x => x.Ticks));
            var transcribedTime = transcribedAudioFiles.OrderByDescending(x => x.EndTime).FirstOrDefault()?.EndTime ?? TimeSpan.Zero;
            Assert.Equal(6, transcribedAudioFiles.Length);
            Assert.Equal(TimeSpan.FromSeconds(300 + (5 * ExtraSeconds)), totalTime);
            Assert.Equal(TimeSpan.FromSeconds(300), transcribedTime);

            CleanData(transcribedAudioFiles);
        }

        [Fact]
        public async Task SplitAudioFileAsync_NotEnoughTime_ReturnsAudioFiles()
        {
            // Arrange
            var subscriptionTime = TimeSpan.FromMinutes(3);

            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var currentUserSubscriptionRepositoryMock = new Mock<ICurrentUserSubscriptionRepository>();
            var loggerMock = new Mock<ILogger>();

            var sampleBytes = await GetSampleBytes();

            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            diskStorageMock.Setup(x => x.GetDirectoryPath(It.IsAny<string>())).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock.Setup(x => x.ReadAllBytesAsync(It.IsAny<string>(), default)).ReturnsAsync(sampleBytes);
            fileAccessServiceMock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());
            currentUserSubscriptionRepositoryMock.Setup(x => x.GetRemainingTimeAsync(It.IsAny<Guid>(), default)).ReturnsAsync(subscriptionTime);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var wavFileService = new WavFileService(
                fileAccessServiceMock.Object,
                blobStorageMock.Object,
                indexMock.Object,
                currentUserSubscriptionRepositoryMock.Object,
                loggerMock.Object);

            // Act
            var transcribedAudioFiles = await wavFileService.SplitAudioFileAsync(new AudioFile(), default);

            // Assert
            var totalTime = TimeSpan.FromTicks(transcribedAudioFiles.Select(x => x.TotalTime).Sum(x => x.Ticks));
            var transcribedTime = transcribedAudioFiles.OrderByDescending(x => x.EndTime).FirstOrDefault()?.EndTime ?? TimeSpan.Zero;
            Assert.Equal(4, transcribedAudioFiles.Length);
            Assert.Equal(TimeSpan.FromSeconds(180 + (3 * ExtraSeconds)), totalTime);
            Assert.Equal(TimeSpan.FromSeconds(180), transcribedTime);

            CleanData(transcribedAudioFiles);
        }

        [Fact]
        public async Task RunConversionToWavAsync_BlobStorageThrowsException_DeleteData()
        {
            // Arrange
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var blobStorageMock = new Mock<IBlobStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var currentUserSubscriptionRepositoryMock = new Mock<ICurrentUserSubscriptionRepository>();
            var loggerMock = new Mock<ILogger>();

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                SourceFileName = "file-name.voc",
                TotalTime = TimeSpan.FromMinutes(5),
                TranscriptionStartTime = TimeSpan.Zero,
                TranscriptionEndTime = TimeSpan.FromMinutes(5)
            };

            blobStorageMock
                .Setup(x => x.GetAsync(It.IsAny<GetBlobSettings>(), default))
                .Throws(new RequestFailedException(string.Empty));
            diskStorageMock.Setup(x => x.GetDirectoryPath(It.IsAny<string>())).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            fileAccessServiceMock.Setup(x => x.Exists(It.Is<string>(n => n == audioFile.SourceFileName))).Returns(false);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var wavFileService = new WavFileService(
                fileAccessServiceMock.Object,
                blobStorageMock.Object,
                indexMock.Object,
                currentUserSubscriptionRepositoryMock.Object,
                loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<RequestFailedException>(async () => await wavFileService.RunConversionToWavAsync(audioFile, default));

            fileAccessServiceMock.Verify(x => x.Delete(It.Is<string>(p => string.IsNullOrEmpty(p))));
        }

        [Theory]
        [InlineData(0, 5, 5)]
        [InlineData(1, 3, 2)]
        public async Task RunConversionToWavAsync_ConversionSuccess(int startTime, int endTime, int totalTime)
        {
            var diskStorageMock = new DiskStorageStub();

            try
            {
                // Arrange
                var fileAccessServiceMock = new Mock<IFileAccessService>();
                var blobStorageMock = new Mock<IBlobStorage>();
                var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
                var currentUserSubscriptionRepositoryMock = new Mock<ICurrentUserSubscriptionRepository>();
                var loggerMock = new Mock<ILogger>();

                var sampleBytes = await GetSampleBytes();

                var audioFile = new AudioFile
                {
                    Id = Guid.NewGuid(),
                    SourceFileName = "file-name.voc",
                    TotalTime = TimeSpan.FromMinutes(5),
                    TranscriptionStartTime = TimeSpan.FromMinutes(startTime),
                    TranscriptionEndTime = TimeSpan.FromMinutes(endTime)
                };

                blobStorageMock
                    .Setup(x => x.GetAsync(It.IsAny<GetBlobSettings>(), default))
                    .ReturnsAsync(sampleBytes);
                indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock);
                fileAccessServiceMock.Setup(x => x.Exists(It.Is<string>(n => n == audioFile.SourceFileName))).Returns(false);
                loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

                var wavFileService = new WavFileService(
                    fileAccessServiceMock.Object,
                    blobStorageMock.Object,
                    indexMock.Object,
                    currentUserSubscriptionRepositoryMock.Object,
                    loggerMock.Object);

                // Act
                var fileName = await wavFileService.RunConversionToWavAsync(audioFile, default);

                // Assert
                var filePath = Path.Combine(diskStorageMock.GetDirectoryPath(), fileName);
                Assert.Equal(TimeSpan.FromMinutes(totalTime), GetAudioFileTotalTime(filePath));
            }
            finally
            {
                diskStorageMock.Clean();
            }
        }

        private Task<byte[]> GetSampleBytes()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var path = Path.Combine(directory, "Samples", "sample.wav");
            return File.ReadAllBytesAsync(path);
        }

        private TimeSpan GetAudioFileTotalTime(string path)
        {
            using (var reader = new MediaFoundationReader(path))
            {
                return reader.TotalTime;
            }
        }

        private void CleanData(TranscribedAudioFile[] transcribedAudioFiles)
        {
            foreach (var transcribedAudioFile in transcribedAudioFiles)
            {
                if (File.Exists(transcribedAudioFile.Path))
                {
                    File.Delete(transcribedAudioFile.Path);
                }
            }
        }
    }
}
