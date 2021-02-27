using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Payloads.ControlPanel;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class CleanUpAudioFilesCommand : CleanUpBaseCommand<CleanUpAudioFilesPayload, CommandResult<CleanUpAudioFilesOutputModel>>, ICleanUpAudioFilesCommand
    {
        private const string RootDirectory = "deleted-audio-files";

        private readonly IPermanentDeleteAllCommand _permanentDeleteAllCommand;

        public CleanUpAudioFilesCommand(
            IPermanentDeleteAllCommand permanentDeleteAllCommand,
            IFileAccessService fileAccessService,
            IZipFileService zipFileService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IAudioFileRepository audioFileRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
            : base(RootDirectory, fileAccessService, zipFileService, blobStorage, index, audioFileRepository, unitOfWork, options, logger.ForContext<CleanUpAudioFilesCommand>())
        {
            _permanentDeleteAllCommand = permanentDeleteAllCommand;
        }

        protected override async Task<CommandResult<CleanUpAudioFilesOutputModel>> Execute(CleanUpAudioFilesPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var deleteBefore = DateTime.UtcNow.AddDays(-30);
            var audioFiles = await AudioFileRepository.GetAllForPermanentDeleteAsync(deleteBefore, cancellationToken);
            if (!audioFiles.Any())
            {
                Logger.Information("No audio files for cleanup");
                return new CommandResult<CleanUpAudioFilesOutputModel>(new CleanUpAudioFilesOutputModel());
            }

            return await ProcessAsync(audioFiles, cancellationToken);
        }

        protected override async Task ProcessAudioFileAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            var permanentDeleteAllPayload = new PermanentDeleteAllPayload(new[] { audioFile.Id }, audioFile.UserId, AppSettings.ApplicationId);
            await _permanentDeleteAllCommand.ExecuteAsync(permanentDeleteAllPayload, null, cancellationToken);
        }
    }
}
