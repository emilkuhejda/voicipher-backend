using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/chunks")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class FileChunkController : ControllerBase
    {
        private readonly Lazy<IUploadFileChunkCommand> _uploadFileChunkCommand;
        private readonly Lazy<IDeleteFileChunkCommand> _deleteFileChunkCommand;
        private readonly Lazy<ISubmitFileChunkCommand> _submitFileChunkCommand;

        public FileChunkController(
            Lazy<IUploadFileChunkCommand> uploadFileChunkCommand,
            Lazy<IDeleteFileChunkCommand> deleteFileChunkCommand,
            Lazy<ISubmitFileChunkCommand> submitFileChunkCommand)
        {
            _uploadFileChunkCommand = uploadFileChunkCommand;
            _deleteFileChunkCommand = deleteFileChunkCommand;
            _submitFileChunkCommand = submitFileChunkCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(StorageConfigurationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetChunksStorageConfiguration")]
        public IActionResult GetChunksStorageConfiguration()
        {
            return Ok(new StorageConfigurationOutputModel(StorageSetting.Azure));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UploadChunkFile")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        public async Task<IActionResult> Upload(Guid fileItemId, int order, Guid applicationId, IFormFile file, CancellationToken cancellationToken)
        {
            var uploadFileChunkPayload = new UploadFileChunkPayload
            {
                AudioFileId = fileItemId,
                Order = order,
                ApplicationId = applicationId,
                File = file
            };

            var commandResult = await _uploadFileChunkCommand.Value.ExecuteAsync(uploadFileChunkPayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpDelete("{fileItemId}")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "DeleteChunks")]
        public async Task<IActionResult> DeleteChunks(Guid fileItemId, Guid applicationId, CancellationToken cancellationToken)
        {
            var deleteFileChunkPayload = new DeleteFileChunkPayload(fileItemId, applicationId);

            var commandResult = await _deleteFileChunkCommand.Value.ExecuteAsync(deleteFileChunkPayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("submit")]
        [ProducesResponseType(typeof(FileItemOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "SubmitChunks")]
        public async Task<IActionResult> Submit(Guid fileItemId, int chunksCount, Guid applicationId, CancellationToken cancellationToken)
        {
            var submitFileChunkPayload = new SubmitFileChunkPayload
            {
                AudioFileId = fileItemId,
                ChunksCount = chunksCount,
                ApplicationId = applicationId
            };

            var commandResult = await _submitFileChunkCommand.Value.ExecuteAsync(submitFileChunkPayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
