using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Queries.TranscribeItems;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Queries.TranscribeItems
{
    public class GetTranscribeItemSourceQuery : Query<Guid, QueryResult<byte[]>>, IGetTranscribeItemSourceQuery
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly ILogger _logger;

        public GetTranscribeItemSourceQuery(
            IBlobStorage blobStorage,
            ITranscribeItemRepository transcribeItemRepository,
            ILogger logger)
        {
            _blobStorage = blobStorage;
            _transcribeItemRepository = transcribeItemRepository;
            _logger = logger.ForContext<GetTranscribeItemSourceQuery>();
        }

        protected override async Task<QueryResult<byte[]>> Execute(Guid parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var transcribeItem = await _transcribeItemRepository.GetAsync(parameter, cancellationToken);
            if (transcribeItem == null)
            {
                _logger.Error($"Transcribe item '{parameter}' was not found.");

                return new QueryResult<byte[]>(new OperationError(ValidationErrorCodes.NotFound));
            }

            try
            {
                var userId = principal.GetNameIdentifier();
                var blobSettings = new GetBlobSettings(transcribeItem.SourceFileName, userId, transcribeItem.AudioFileId);
                var blobItem = await _blobStorage.GetAsync(blobSettings, cancellationToken);

                _logger.Information($"Blob file '{transcribeItem.SourceFileName}' was downloaded from blob storage. Audio file ID = {transcribeItem.AudioFileId}, Transcribe item ID = {transcribeItem.Id}.");

                return new QueryResult<byte[]>(blobItem);
            }
            catch (RequestFailedException ex)
            {
                _logger.Warning(ex, $"Cannot connect to blob storage. Audio file ID = {transcribeItem.AudioFileId}, Transcribe item ID = {transcribeItem.Id}, Transcribe file name = {transcribeItem.SourceFileName}.");

                return new QueryResult<byte[]>(new byte[0]);
            }
            catch (BlobNotExistsException)
            {
                _logger.Warning($"Blob file '{transcribeItem.SourceFileName}' not found. Audio file ID = {transcribeItem.AudioFileId}, Transcribe item ID = {transcribeItem.Id}.");

                return new QueryResult<byte[]>(new byte[0]);
            }
        }
    }
}
