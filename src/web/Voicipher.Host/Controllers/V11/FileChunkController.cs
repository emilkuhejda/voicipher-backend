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
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V11
{
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiExplorerSettings(GroupName = "v1.1")]
    [Route("api/v{version:apiVersion}/chunks")]
    [Produces("application/json")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class FileChunkController : ControllerBase
    {
        private readonly Lazy<IUploadFileChunkCommand> _uploadFileChunkCommand;

        public FileChunkController(Lazy<IUploadFileChunkCommand> uploadFileChunkCommand)
        {
            _uploadFileChunkCommand = uploadFileChunkCommand;
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
        public async Task<IActionResult> Upload(Guid fileItemId, int order, StorageSetting storageSetting, Guid applicationId, IFormFile file, CancellationToken cancellationToken)
        {
            var uploadFileChunkPayload = new UploadFileChunkPayload
            {
                FileItemId = fileItemId,
                Order = order,
                StorageSetting = storageSetting,
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
            var deleteFileChunk = new DeleteFileChunkPayload(fileItemId, applicationId);
        }
    }
}
