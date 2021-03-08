using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Voicipher.Business.StateMachine;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Settings;
using Xunit;

namespace Voicipher.Business.Tests.StateMachine
{
    public class JobStateMachineTests
    {
        [Fact]
        public async Task DoInitAsync_InitializesMachineState()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

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
                Mock.Of<IIndex<StorageLocation, IDiskStorage>>(),
                Mock.Of<IAudioFileRepository>(),
                Mock.Of<ITranscribeItemRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IOptions<AppSettings>>(),
                loggerMock.Object);

            // Act

            // Assert
        }
    }
}
