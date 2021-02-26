using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
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
    public class CleanUpAudioFilesCommand : Command<CleanUpAudioFilesPayload, CommandResult<CleanUpAudioFilesOutputModel>>, ICleanUpAudioFilesCommand
    {
        private readonly IPermanentDeleteAllCommand _permanentDeleteAllCommand;
        private readonly IFileAccessService _fileAccessService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public CleanUpAudioFilesCommand(
            IPermanentDeleteAllCommand permanentDeleteAllCommand,
            IFileAccessService fileAccessService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IAudioFileRepository audioFileRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _permanentDeleteAllCommand = permanentDeleteAllCommand;
            _fileAccessService = fileAccessService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Backup];
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
            _appSettings = options.Value;
            _logger = logger.ForContext<CleanUpAudioFilesCommand>();
        }

        protected override async Task<CommandResult<CleanUpAudioFilesOutputModel>> Execute(CleanUpAudioFilesPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var audioFiles = await _audioFileRepository.GetAllForCleanUpAsync(cancellationToken);
            if (!audioFiles.Any())
                return new CommandResult<CleanUpAudioFilesOutputModel>(new CleanUpAudioFilesOutputModel());

            foreach (var group in audioFiles.GroupBy(x => x.UserId))
            {
                var rootDirectory = Path.Combine("audio-files", group.Key.ToString());
                var rootPath = _diskStorage.GetDirectoryPath(rootDirectory);
                _fileAccessService.DeleteDirectory(rootPath);

                using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
                {
                    foreach (var audioFile in group)
                    {
                        try
                        {
                            var folderPath = Path.Combine(rootDirectory, audioFile.Id.ToString());
                            await BackupSourceAsync(new BackupSourceSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);
                            await BackupSourceAsync(new BackupSourceSettings(audioFile.SourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);

                            foreach (var transcribeItem in audioFile.TranscribeItems)
                            {
                                await BackupSourceAsync(new BackupSourceSettings(transcribeItem.SourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);
                            }

                            var permanentDeleteAllPayload = new PermanentDeleteAllPayload(new[] { audioFile.Id }, audioFile.UserId, _appSettings.ApplicationId);
                            await _permanentDeleteAllCommand.ExecuteAsync(permanentDeleteAllPayload, principal, cancellationToken);

                            var serializedAudioFile = JsonConvert.SerializeObject(audioFile);
                            var jsonBytes = Encoding.UTF8.GetBytes(serializedAudioFile);
                            await _diskStorage.UploadAsync(jsonBytes, new UploadSettings(folderPath, $"{audioFile.Id}.json"), cancellationToken);

                            await transaction.CommitAsync(cancellationToken);
                        }
                        catch (RequestFailedException ex)
                        {
                            _logger.Error(ex, "Blob storage is unavailable");
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"Backup process for audio file {audioFile.Id} failed");
                        }
                    }
                }
            }

            return new CommandResult<CleanUpAudioFilesOutputModel>(new CleanUpAudioFilesOutputModel());
        }

        private async Task BackupSourceAsync(BackupSourceSettings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.FileName))
                return;

            try
            {
                var blobSettings = new GetBlobSettings(settings.FileName, settings.UserId, settings.AudioFileId);
                var source = await _blobStorage.GetAsync(blobSettings, cancellationToken);

                var uploadSettings = new UploadSettings(settings.FolderName, settings.FileName);
                await _diskStorage.UploadAsync(source, uploadSettings, cancellationToken);
            }
            catch (BlobNotExistsException)
            {
                _logger.Information($"File {settings.FileName} not exists in the blob storage. User ID = {settings.UserId}, Audio file ID = {settings.AudioFileId}");
            }
        }

        private record BackupSourceSettings
        {
            public BackupSourceSettings(string fileName, Guid userId, Guid audioFileId, string folderName)
            {
                FileName = fileName;
                UserId = userId;
                AudioFileId = audioFileId;
                FolderName = folderName;
            }

            public string FileName { get; }

            public Guid UserId { get; }

            public Guid AudioFileId { get; }

            public string FolderName { get; }
        }
    }
}
