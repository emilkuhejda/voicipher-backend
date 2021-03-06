using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Google.Protobuf.Collections;
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
using Voicipher.Domain.Payloads;
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
            await runBackgroundJobCommand.ExecuteAsync(new BackgroundJobPayload(), null, default);
        }

        private IJobStateMachine CreateJobStateMachineMock()
        {
            var canRunRecognitionCommandMock = new Mock<ICanRunRecognitionCommand>();
            var wavFileServiceMock = new Mock<IWavFileService>();
            var audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            canRunRecognitionCommandMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CanRunRecognitionPayload>(), null, default))
                .ReturnsAsync(new CommandResult());
            wavFileServiceMock
                .Setup(x => x.SplitAudioFileAsync(It.IsAny<AudioFile>(), default))
                .ReturnsAsync(new[] { new TranscribedAudioFile() });
            audioFileRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(new AudioFile());

            return new JobStateMachine(
                canRunRecognitionCommandMock.Object,
                Mock.Of<IModifySubscriptionTimeCommand>(),
                Mock.Of<IUpdateRecognitionStateCommand>(),
                wavFileServiceMock.Object,
                Mock.Of<ISpeechRecognitionService>(),
                Mock.Of<IMessageCenterService>(),
                Mock.Of<IBlobStorage>(),
                Mock.Of<IAudioFileRepository>(),
                Mock.Of<ITranscribeItemRepository>(),
                unitOfWorkMock.Object,
                Mock.Of<IOptions<AppSettings>>(),
                Mock.Of<ILogger>());
        }

        private async Task<RepeatedField<SpeechRecognitionResult>> GetSpeechRecognitionResults()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var pathToJson = Path.Combine(directory, "JsonData", "alternatives.json");
            var jsonText = await File.ReadAllTextAsync(pathToJson);

            return JsonConvert.DeserializeObject<RepeatedField<SpeechRecognitionResult>>(jsonText);
        }
    }
}
