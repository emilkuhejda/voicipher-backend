using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.ControlPanel;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class ImportAudioFileCommand : Command<ImportAudioFilePayload, CommandResult<int>>, IImportAudioFileCommand
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public ImportAudioFileCommand(IBlobStorage blobStorage, IUserRepository userRepository, IUnitOfWork unitOfWork, IOptions<AppSettings> options, ILogger logger)
        {
            _blobStorage = blobStorage;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.ForContext<ImportAudioFileCommand>();
            _appSettings = options.Value;
        }

        protected override async Task<CommandResult<int>> Execute(ImportAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var usersJson = await File.ReadAllTextAsync(parameter.UsersJsonPath, cancellationToken);
            var subscriptionsJson = await File.ReadAllTextAsync(parameter.SubscriptionsJsonPath, cancellationToken);
            var alternativesJson = await File.ReadAllTextAsync(parameter.AlternativesJsonPath, cancellationToken);

            var alternatives = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(alternativesJson);
            var users = JsonConvert.DeserializeObject<User[]>(usersJson);
            var subscriptions = JsonConvert.DeserializeObject<CurrentUserSubscription[]>(subscriptionsJson);

            _logger.Information("Start importing data");

            using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
            {
                foreach (var user in users)
                {
                    var currentSubscription = subscriptions.SingleOrDefault(x => x.UserId == user.Id);
                    if (currentSubscription != null)
                    {
                        user.CurrentUserSubscription = currentSubscription;
                    }

                    foreach (var audioFile in user.AudioFiles)
                    {
                        if (audioFile.IsPermanentlyDeleted)
                            continue;

                        audioFile.Storage = StorageSetting.Azure;
                        audioFile.DateUpdatedUtc = DateTime.UtcNow;
                        audioFile.ApplicationId = _appSettings.ApplicationId;

                        var rootPath = Path.Combine(parameter.UploadsDirectoryPath, audioFile.UserId.ToString(), audioFile.Id.ToString());

                        if (audioFile.RecognitionState != RecognitionState.Completed)
                        {
                            if (!string.IsNullOrWhiteSpace(audioFile.OriginalSourceFileName))
                            {
                                var filePath = Path.Combine(rootPath, "source", audioFile.OriginalSourceFileName);
                                if (File.Exists(filePath))
                                {
                                    var uploadSettings = new UploadBlobSettings(filePath, audioFile.UserId, audioFile.Id);
                                    var name = await _blobStorage.UploadAsync(uploadSettings, cancellationToken);
                                    audioFile.OriginalSourceFileName = name;

                                    _logger.Information($"Original audio file {name} source was uploaded to storage");
                                }
                                else
                                {
                                    audioFile.OriginalSourceFileName = string.Empty;
                                }
                            }
                            else
                            {
                                audioFile.OriginalSourceFileName = string.Empty;
                            }

                            if (!string.IsNullOrWhiteSpace(audioFile.SourceFileName))
                            {
                                var filePath = Path.Combine(rootPath, "source", audioFile.SourceFileName);
                                if (File.Exists(filePath))
                                {
                                    var uploadSettings = new UploadBlobSettings(filePath, audioFile.UserId, audioFile.Id);
                                    var name = await _blobStorage.UploadAsync(uploadSettings, cancellationToken);
                                    audioFile.SourceFileName = name;

                                    _logger.Information($"Audio file source {name} was uploaded to storage");
                                }
                                else
                                {
                                    audioFile.SourceFileName = string.Empty;
                                }
                            }
                            else
                            {
                                audioFile.SourceFileName = string.Empty;
                            }
                        }
                        else
                        {
                            audioFile.OriginalSourceFileName = string.Empty;
                            audioFile.SourceFileName = string.Empty;
                        }

                        foreach (var transcribeItem in audioFile.TranscribeItems)
                        {
                            transcribeItem.AudioFileId = audioFile.Id;
                            if (alternatives.ContainsKey(transcribeItem.Id))
                            {
                                if (!string.IsNullOrWhiteSpace(alternatives[transcribeItem.Id]))
                                {
                                    var serialize = JsonConvert.DeserializeObject<IEnumerable<RecognitionAlternative>>(alternatives[transcribeItem.Id]);
                                    transcribeItem.Alternatives = JsonConvert.SerializeObject(serialize);
                                }
                                transcribeItem.Storage = StorageSetting.Azure;
                            }

                            var path = Path.Combine(rootPath, "transcriptions", transcribeItem.SourceFileName);
                            if (File.Exists(path))
                            {
                                var fileName = $"{transcribeItem.SourceFileName}.voc";
                                var metadata = new Dictionary<string, string> { { BlobMetadata.TranscribedAudioFile, true.ToString() } };
                                var uploadSettings = new UploadBlobSettings(path, audioFile.UserId, audioFile.Id, fileName, metadata);
                                var name = await _blobStorage.UploadAsync(uploadSettings, cancellationToken);
                                transcribeItem.SourceFileName = name;

                                _logger.Information($"Transcription audio file source {fileName} was uploaded to storage");
                            }
                            else
                            {
                                transcribeItem.SourceFileName = string.Empty;
                            }
                        }
                    }

                    await _userRepository.AddAsync(user);
                    await _unitOfWork.SaveAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }

            return new CommandResult<int>(users.Length);
        }
    }
}
