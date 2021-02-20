using System;
using System.IO;
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

            var sampleBytes = await File.ReadAllBytesAsync(@"C:\Users\kuem\Downloads\samples\spkr.wav");

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
        }
    }
}
