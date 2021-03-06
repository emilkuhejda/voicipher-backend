using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Commands.Job;
using Voicipher.Business.StateMachine;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Commands.Notifications;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Queries.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Payloads.ControlPanel;
using Voicipher.Domain.Payloads.Job;
using Voicipher.Domain.Settings;
using Xunit;

namespace Voicipher.Business.Tests.Commands
{
    public class RunBackgroundJobCommandTests
    {
        [Fact]
        public async Task ExecuteCommand()
        {
            // Arrange
            var createInformationMessageCommandMock = new Mock<ICreateInformationMessageCommand>();
            var deleteAudioFileSourceCommandMock = new Mock<IDeleteAudioFileSourceCommand>();
            var getInternalValueQueryMock = new Mock<IGetInternalValueQuery<bool>>();
            var notificationServiceMock = new Mock<INotificationService>();
            var jobStateMachineMock = CreateJobStateMachineMock();
            var backgroundJobRepositoryMock = new Mock<IBackgroundJobRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger>();

            var dbContextTransaction = new Mock<IDbContextTransaction>();

            deleteAudioFileSourceCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<DeleteAudioFileSourcePayload>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult());
            getInternalValueQueryMock
                .Setup(x => x.ExecuteAsync(It.IsAny<InternalValuePayload<bool>>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResult<InternalValueOutputModel<bool>>(new InternalValueOutputModel<bool>(false)));
            unitOfWorkMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(dbContextTransaction.Object);

            var backgroundJob = new BackgroundJob
            {
                Parameters = JsonConvert.SerializeObject(new Dictionary<BackgroundJobParameter, object>()),
                DateCompletedUtc = DateTime.UtcNow,
                DateCreatedUtc = DateTime.UtcNow
            };

            backgroundJobRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), default)).ReturnsAsync(backgroundJob);
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            var runBackgroundJobCommand = new RunBackgroundJobCommand(
                createInformationMessageCommandMock.Object,
                deleteAudioFileSourceCommandMock.Object,
                getInternalValueQueryMock.Object,
                notificationServiceMock.Object,
                jobStateMachineMock,
                backgroundJobRepositoryMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object);

            // Act
            var commandResult = await runBackgroundJobCommand.ExecuteAsync(new BackgroundJobPayload(), null, default);

            // Assert
            Assert.True(commandResult.IsSuccess);

            dbContextTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()));
        }

        private IJobStateMachine CreateJobStateMachineMock()
        {
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var modifySubscriptionTimeCommandMock = new Mock<IModifySubscriptionTimeCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var diskStorageMock = new Mock<IDiskStorage>();
            var indexMock = new Mock<IIndex<StorageLocation, IDiskStorage>>();
            var fileAccessServiceMock = new Mock<IFileAccessService>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var optionsMock = new Mock<IOptions<AppSettings>>();
            var loggerMock = new Mock<ILogger>();

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            modifySubscriptionTimeCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<ModifySubscriptionTimePayload>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult());
            fileAccessServiceMock
                .Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);
            wavFileServiceMock
                .Setup(x => x.SplitAudioFileAsync(It.IsAny<AudioFile>(), default))
                .ReturnsAsync(new[] { new TranscribedAudioFile() });
            indexMock
                .Setup(x => x[It.IsAny<StorageLocation>()])
                .Returns(diskStorageMock.Object);
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AudioFile());
            optionsMock.Setup(x => x.Value).Returns(new AppSettings());
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(Mock.Of<ILogger>());

            return new JobStateMachine(
                canRunRecognitionCommandMock.Object,
                modifySubscriptionTimeCommandMock.Object,
                Mock.Of<IUpdateRecognitionStateCommand>(),
                wavFileServiceMock.Object,
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                Mock.Of<IBlobStorage>(),
                indexMock.Object,
                fileAccessServiceMock.Object,
                audioFileRepositoryMock.Object,
                Mock.Of<ITranscribeItemRepository>(),
                unitOfWorkMock.Object,
                optionsMock.Object,
                loggerMock.Object);
        }
    }
}
