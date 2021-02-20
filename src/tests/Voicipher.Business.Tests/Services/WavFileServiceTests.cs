using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Moq;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Xunit;

namespace Voicipher.Business.Tests.Services
{
    public class WavFileServiceTests
    {
        [Fact]
        public async Task SplitAudioFileAsync_ReturnsAudioFiles()
        {
            // Arrange
            var blobStorageMock = new Mock<IBlobStorage>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var currentUserSubscriptionRepositoryMock = new Mock<ICurrentUserSubscriptionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger>();

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var path = Path.Combine(directory, "Samples", "sample.wav");
            var sampleBytes = await File.ReadAllBytesAsync(path);

            diskStorageMock.Setup(x => x.GetDirectoryPath()).Returns(string.Empty);
            indexMock.Setup(x => x[It.IsAny<StorageLocation>()]).Returns(diskStorageMock.Object);
            fileAccessServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            fileAccessServiceMock.Setup(x => x.ReadAllBytesAsync(It.IsAny<string>(), default)).ReturnsAsync(sampleBytes);
            currentUserSubscriptionRepositoryMock.Setup(x => x.GetRemainingTimeAsync(It.IsAny<Guid>(), default)).ReturnsAsync(TimeSpan.FromMinutes(10));

            var wavFileService = new WavFileService(
                blobStorageMock.Object,
                indexMock.Object,
                fileAccessServiceMock.Object,
                currentUserSubscriptionRepositoryMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object);

            // Act
            var transcribedAudioFiles = await wavFileService.SplitAudioFileAsync(new AudioFile(), default);

            // Assert
            var totalTime = TimeSpan.FromTicks(transcribedAudioFiles.Select(x => x.TotalTime).Sum(x => x.Ticks));
            Assert.Equal(6, transcribedAudioFiles.Length);
            Assert.Equal(TimeSpan.FromSeconds(302.5), totalTime);
            Assert.Equal(TimeSpan.FromSeconds(300), transcribedAudioFiles.OrderByDescending(x => x.EndTime).FirstOrDefault()?.EndTime ?? TimeSpan.Zero);
        }
    }
}
