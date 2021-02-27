using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.ControlPanel
{
    public abstract class CleanUpBaseCommand<T, TResult> : Command<T, TResult>
    {
        private readonly string _rootDirectory;
        private readonly IFileAccessService _fileAccessService;
        private readonly IZipFileService _zipFileService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;

        protected CleanUpBaseCommand(
            string rootDirectory,
            IFileAccessService fileAccessService,
            IZipFileService zipFileService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _rootDirectory = rootDirectory;
            _fileAccessService = fileAccessService;
            _zipFileService = zipFileService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Backup];
            UnitOfWork = unitOfWork;
            AppSettings = options.Value;
            Logger = logger;
        }

        protected IUnitOfWork UnitOfWork { get; }

        protected AppSettings AppSettings { get; }

        protected ILogger Logger { get; }

        protected async Task<CommandResult<CleanUpAudioFilesOutputModel>> ProcessAsync(AudioFile[] audioFiles, CancellationToken cancellationToken)
        {
            var rootPath = _diskStorage.GetDirectoryPath(_rootDirectory);

            var succeededIds = new Dictionary<Guid, IList<Guid>>();
            var failedIds = new Dictionary<Guid, IList<Guid>>();

            Logger.Information($"There was found {audioFiles} audio files for cleanup");

            foreach (var group in audioFiles.GroupBy(x => x.UserId))
            {
                var userId = group.Key;
                succeededIds.Add(userId, new List<Guid>());
                failedIds.Add(userId, new List<Guid>());

                Logger.Information($"Start cleanup for user ID {userId}");

                var userRootPath = Path.Combine(rootPath, userId.ToString());
                _fileAccessService.DeleteDirectory(userRootPath);


                foreach (var audioFile in group)
                {
                    using (var transaction = await UnitOfWork.BeginTransactionAsync(cancellationToken))
                    {
                        Logger.Verbose($"Start cleanup of the audio file ID {audioFile.Id}");

                        try
                        {
                            var folderPath = Path.Combine(_rootDirectory, userId.ToString(), audioFile.Id.ToString());

                            var jsonSerializerSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                            var serializedAudioFile = JsonConvert.SerializeObject(audioFile, Formatting.None, jsonSerializerSettings);
                            var jsonBytes = Encoding.UTF8.GetBytes(serializedAudioFile);
                            var jsonPath = await _diskStorage.UploadAsync(jsonBytes, new UploadSettings(folderPath, $"{audioFile.Id}.json"), cancellationToken);
                            Logger.Verbose($"Json for audio file {audioFile.Id} was created on destination {jsonPath}");

                            await BackupSourceAsync(new BackupSourceSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);
                            await BackupSourceAsync(new BackupSourceSettings(audioFile.SourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);

                            foreach (var transcribeItem in audioFile.TranscribeItems)
                            {
                                await BackupSourceAsync(new BackupSourceSettings(transcribeItem.SourceFileName, audioFile.UserId, audioFile.Id, folderPath), cancellationToken);
                            }

                            await ProcessAudioFileAsync(audioFile, cancellationToken);
                            await transaction.CommitAsync(cancellationToken);

                            Logger.Information($"Audio file {audioFile.Id} was successfully cleaned up");

                            succeededIds[userId].Add(audioFile.Id);
                        }
                        catch (RequestFailedException ex)
                        {
                            Logger.Error(ex, "Blob storage is unavailable");
                            failedIds[userId].Add(audioFile.Id);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"Cleanup process for audio file {audioFile.Id} failed");
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

        protected abstract Task ProcessAudioFileAsync(AudioFile audioFile, CancellationToken cancellationToken);

        protected void CompressData(Guid userId, string rootPath, string sourcePath)
        {
            if (!_fileAccessService.DirectoryExists(sourcePath))
                return;

            try
            {
                Logger.Verbose($"[{userId}] Start compressing user data");
                var destinationFileName = Path.Combine(rootPath, $"{userId}-{DateTime.UtcNow}.zip");
                _zipFileService.CreateFromDirectory(sourcePath, destinationFileName);
                Logger.Information($"[{userId}] User data was compressed to zip file in the destination {destinationFileName}");

                _fileAccessService.DeleteDirectory(sourcePath);
                Logger.Information($"[{userId}] User data was deleted from dist storage");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"[{userId}] Compression failed");
            }
        }

        protected async Task BackupSourceAsync(BackupSourceSettings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.FileName))
                return;

            try
            {
                Logger.Verbose($"Start downloading audio source {settings.FileName}");
                var blobSettings = new GetBlobSettings(settings.FileName, settings.UserId, settings.AudioFileId);
                var source = await _blobStorage.GetAsync(blobSettings, cancellationToken);
                Logger.Verbose($"Audio source {settings.FileName} was downloaded");

                var uploadSettings = new UploadSettings(settings.FolderName, settings.FileName);
                var sourcePath = await _diskStorage.UploadAsync(source, uploadSettings, cancellationToken);
                Logger.Verbose($"Audio source {settings.FileName} was uploaded to disk storage on destination {sourcePath}");
            }
            catch (BlobNotExistsException)
            {
                Logger.Error($"File {settings.FileName} not exists in the blob storage. User ID = {settings.UserId}, Audio file ID = {settings.AudioFileId}");
            }
        }

        protected record BackupSourceSettings
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
