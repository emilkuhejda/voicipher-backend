using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Moq;
using Serilog;
using Voicipher.Business.Services;
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

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var path = Path.Combine(directory, "Samples", "sample.wav");
            var sampleBytes = await File.ReadAllBytesAsync(path);

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

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var path = Path.Combine(directory, "Samples", "sample.wav");
            var sampleBytes = await File.ReadAllBytesAsync(path);

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
        }
    }
}
