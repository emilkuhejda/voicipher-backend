using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class CleanUpTranscribeItemsCommand : CleanUpBaseCommand<CleanUpTranscribeItemsPayload, CommandResult<CleanUpAudioFilesOutputModel>>, ICleanUpTranscribeItemsCommand
    {
        private const string RootDirectory = "cleaned-audio-files";

        private readonly IAudioFileRepository _audioFileRepository;

        public CleanUpTranscribeItemsCommand(
            IAudioFileRepository audioFileRepository,
            IFileAccessService fileAccessService,
            IZipFileService zipFileService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
            : base(RootDirectory, fileAccessService, zipFileService, blobStorage, index, unitOfWork, options, logger.ForContext<CleanUpTranscribeItemsCommand>())
        {
            _audioFileRepository = audioFileRepository;
        }

        protected override async Task<CommandResult<CleanUpAudioFilesOutputModel>> Execute(CleanUpTranscribeItemsPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var deleteBefore = DateTime.UtcNow.AddDays(-60);
            var audioFiles = await _audioFileRepository.GetAllForCleanUpAsync(deleteBefore, cancellationToken);
            if (!audioFiles.Any())
            {
                Logger.Information("No audio files for cleanup");
                return new CommandResult<CleanUpAudioFilesOutputModel>(new CleanUpAudioFilesOutputModel());
            }

            return await ProcessAsync(audioFiles, cancellationToken);
        }

        protected override async Task ProcessAudioFileAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            foreach (var transcribeItem in audioFile.TranscribeItems)
            {
                transcribeItem.ApplicationId = AppSettings.ApplicationId;
                transcribeItem.DateUpdatedUtc = DateTime.UtcNow;
                transcribeItem.WasCleaned = true;

                await UnitOfWork.SaveAsync(cancellationToken);
            }

            audioFile.ApplicationId = AppSettings.ApplicationId;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;
            audioFile.WasCleaned = true;
            await UnitOfWork.SaveAsync(cancellationToken);

            try
            {
                Logger.Verbose($"[{audioFile.UserId}] Start deleting audio file {audioFile.Id}");
                await BlobStorage.DeleteAudioFileAsync(new BlobSettings(audioFile.Id, audioFile.UserId), cancellationToken);
                Logger.Verbose($"[{audioFile.UserId}] Delete audio file {audioFile.Id} from blob storage");
            }
            catch (RequestFailedException ex)
            {
                Logger.Error(ex, $"[{audioFile.UserId}] Blob storage is unavailable. Audio file {audioFile.Id}");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"[{audioFile.UserId}] Delete audio file for blob storage failed");
            }
        }
    }
}
