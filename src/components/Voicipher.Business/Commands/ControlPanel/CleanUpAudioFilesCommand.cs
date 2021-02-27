using System;
using System.Collections.Generic;
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
using Voicipher.Business.Services;
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
        private const string RootDirectory = "audio-files";

        private readonly IPermanentDeleteAllCommand _permanentDeleteAllCommand;
        private readonly IFileAccessService _fileAccessService;
        private readonly IZipFileService _zipFileService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

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
        {
            _permanentDeleteAllCommand = permanentDeleteAllCommand;
            _fileAccessService = fileAccessService;
            _zipFileService = zipFileService;
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

            var rootPath = _diskStorage.GetDirectoryPath(RootDirectory);

            var succeededIds = new Dictionary<Guid, IList<Guid>>();
            var failedIds = new Dictionary<Guid, IList<Guid>>();

            _logger.Information($"There was found {audioFiles} audio files for backup");

            foreach (var group in audioFiles.GroupBy(x => x.UserId))
            {
                var userId = group.Key;
                succeededIds.Add(userId, new List<Guid>());
                failedIds.Add(userId, new List<Guid>());

                _logger.Information($"Start backup for user ID {userId}");

                var userRootPath = Path.Combine(rootPath, userId.ToString());
                _fileAccessService.DeleteDirectory(userRootPath);


                foreach (var audioFile in group)
                {
                    using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
                    {
                        _logger.Verbose($"Start backup of the audio file ID {audioFile.Id}");

                        try
                        {
                            var folderPath = Path.Combine(RootDirectory, userId.ToString(), audioFile.Id.ToString());

                            var jsonSerializerSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                            var serializedAudioFile = JsonConvert.SerializeObject(audioFile, Formatting.None, jsonSerializerSettings);
                            var jsonBytes = Encoding.UTF8.GetBytes(serializedAudioFile);
                            var jsonPath = await _diskStorage.UploadAsync(jsonBytes, new UploadSettings(folderPath, $"{audioFile.Id}.json"), cancellationToken);
                            _logger.Verbose($"Json for audio file {audioFile.Id} was created on destination {jsonPath}");

                            await BackupSourceAsync(new BackupSourceSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);
                            await BackupSourceAsync(new BackupSourceSettings(audioFile.SourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);

                            foreach (var transcribeItem in audioFile.TranscribeItems)
                            {
                                await BackupSourceAsync(new BackupSourceSettings(transcribeItem.SourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);
                            }

                            var permanentDeleteAllPayload = new PermanentDeleteAllPayload(new[] { audioFile.Id }, audioFile.UserId, _appSettings.ApplicationId);
                            await _permanentDeleteAllCommand.ExecuteAsync(permanentDeleteAllPayload, principal, cancellationToken);

                            await transaction.CommitAsync(cancellationToken);

                            _logger.Information($"Audio file {audioFile.Id} was successfully backed up");

                            succeededIds[userId].Add(audioFile.Id);
                        }
                        catch (RequestFailedException ex)
                        {
                            _logger.Error(ex, "Blob storage is unavailable");
                            failedIds[userId].Add(audioFile.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"Backup process for audio file {audioFile.Id} failed");
                            failedIds[userId].Add(audioFile.Id);
                        }
                    }
                }

                if (succeededIds[userId].Any())
                {
                    CompressData(userId, rootPath, userRootPath);
                }
            }

            var cleanUpAudioFilesOutputModel = new CleanUpAudioFilesOutputModel(audioFiles.Length, succeededIds, failedIds);
            return new CommandResult<CleanUpAudioFilesOutputModel>(cleanUpAudioFilesOutputModel);
        }

        private void CompressData(Guid userId, string rootPath, string sourcePath)
        {
            if (!_fileAccessService.DirectoryExists(sourcePath))
                return;

            try
            {
                _logger.Verbose($"[{userId}] Start compressing user data");
                var destinationFileName = Path.Combine(rootPath, $"{userId}.zip");
                _zipFileService.CreateFromDirectory(sourcePath, destinationFileName);
                _logger.Information($"[{userId}] User data was compressed to zip file in the destination {destinationFileName}");

                _fileAccessService.DeleteDirectory(sourcePath);
                _logger.Information($"[{userId}] User data was deleted from dist storage");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{userId}] Compression failed");
            }
        }

        private async Task BackupSourceAsync(BackupSourceSettings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.FileName))
                return;

            try
            {
                _logger.Verbose($"Start downloading audio source {settings.FileName}");
                var blobSettings = new GetBlobSettings(settings.FileName, settings.UserId, settings.AudioFileId);
                var source = await _blobStorage.GetAsync(blobSettings, cancellationToken);
                _logger.Verbose($"Audio source {settings.FileName} was downloaded");

                var uploadSettings = new UploadSettings(settings.FolderName, settings.FileName);
                var sourcePath = await _diskStorage.UploadAsync(source, uploadSettings, cancellationToken);
                _logger.Verbose($"Audio source {settings.FileName} was uploaded to disk storage on destination {sourcePath}");
            }
            catch (BlobNotExistsException)
            {
                _logger.Error($"File {settings.FileName} not exists in the blob storage. User ID = {settings.UserId}, Audio file ID = {settings.AudioFileId}");
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
