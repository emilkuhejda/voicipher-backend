using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Common.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UploadFileChunkCommand : Command<UploadFileChunkPayload, CommandResult<OkOutputModel>>, IUploadFileChunkCommand
    {
        private readonly IDiskStorage _diskStorage;
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UploadFileChunkCommand(
            IIndex<StorageLocation, IDiskStorage> index,
            IFileChunkRepository fileChunkRepository,
            IMapper mapper,
            ILogger logger)
        {
            _diskStorage = index[StorageLocation.Chunk];
            _fileChunkRepository = fileChunkRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UploadFileChunkCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(UploadFileChunkPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UploadFileChunkPayload.File), ValidationErrorCodes.ParameterIsNull))
                {
                    _logger.Error("Uploaded file source was not found.");

                    throw new OperationErrorException(ErrorCode.EC100);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var filePath = string.Empty;
            try
            {
                var uploadedFileSource = await parameter.File.GetBytesAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                filePath = await _diskStorage.UploadAsync(uploadedFileSource, cancellationToken);
                _logger.Information($"File chunk was created on destination: {filePath}");

                var fileChunk = _mapper.Map<FileChunk>(
                    parameter,
                    opts => opts.AfterMap((_, f) => f.Path = filePath));

                if (!fileChunk.Validate().IsValid)
                {
                    _logger.Error("Invalid input data for file chunk entity.");

                    throw new OperationErrorException(ErrorCode.EC600);
                }

                await _fileChunkRepository.AddAsync(fileChunk);
                await _fileChunkRepository.SaveAsync(cancellationToken);

                _logger.Information($"File chunk for audio file '{parameter.AudioFileId}' was uploaded");

                return new CommandResult<OkOutputModel>(new OkOutputModel());
            }
            catch (OperationCanceledException)
            {
                _logger.Information("Operation was cancelled");

                throw new OperationErrorException(ErrorCode.EC800);
            }
            catch (Exception ex)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    _logger.Information($"File chunk was removed on destination: {filePath}");
                }

                _logger.Error("File chunk was not uploaded correctly.");
                _logger.Error(ExceptionFormatter.FormatException(ex));

                throw;
            }
        }
    }
}
