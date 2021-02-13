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

namespace Voicipher.Host.Controllers.V2
{
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v{version:apiVersion}/chunks")]
    [Produces("application/json")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class FileChunkController : ControllerBase
    {
        private readonly Lazy<IUploadChunkFileCommand> _uploadChunkFileCommand;

        public FileChunkController(Lazy<IUploadChunkFileCommand> uploadChunkFileCommand)
        {
            _uploadChunkFileCommand = uploadChunkFileCommand;
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

        [AllowAnonymous]
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
            var uploadChunkFileInputModel = new UploadChunkFilePayload
            {
                FileItemId = fileItemId,
                Order = order,
                StorageSetting = storageSetting,
                ApplicationId = applicationId,
                File = file
            };

            var commandResult = await _uploadChunkFileCommand.Value.ExecuteAsync(uploadChunkFileInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
