using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Common.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class DeleteFileChunkCommand : Command<DeleteFileChunkPayload, CommandResult<OkOutputModel>>, IDeleteFileChunkCommand
    {
        private readonly IDiskStorage _diskStorage;
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly ILogger _logger;

        public DeleteFileChunkCommand(
            IIndex<StorageLocation, IDiskStorage> index,
            IFileChunkRepository fileChunkRepository,
            ILogger logger)
        {
            _diskStorage = index[StorageLocation.Chunk];
            _fileChunkRepository = fileChunkRepository;
            _logger = logger.ForContext<DeleteFileChunkCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(DeleteFileChunkPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            try
            {
                var fileChunks = await _fileChunkRepository.GetByAudioFileIdAsync(parameter.AudioFileId);

                _diskStorage.RemoveRange(fileChunks);
                _fileChunkRepository.RemoveRange(fileChunks);
                await _fileChunkRepository.SaveAsync(cancellationToken);

                _logger.Information($"File chunks ({fileChunks.Length}) were deleted for audio file {parameter.AudioFileId}");

                return new CommandResult<OkOutputModel>(new OkOutputModel());
            }
            catch (Exception ex)
            {
                _logger.Error("Operation error.");
                _logger.Error(ExceptionFormatter.FormatException(ex));

                throw new OperationErrorException(ErrorCode.EC603);
            }
        }
    }
}
